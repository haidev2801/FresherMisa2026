param(
    [string]$BaseUrl = "http://localhost:5237/api",
    [ValidateSet("departments", "employees", "positions")]
    [string]$Entity = "departments",
    [int]$ColdRuns = 1,
    [int]$WarmupRuns = 3,
    [int]$HotRuns = 30,
    [int]$TimeoutSec = 30,
    [switch]$SkipById
)

$ErrorActionPreference = "Stop"

function Get-StatSummary {
    param([double[]]$Values)

    if (-not $Values -or $Values.Count -eq 0) {
        return [PSCustomObject]@{
            Count = 0
            MinMs = 0
            MaxMs = 0
            AvgMs = 0
            P95Ms = 0
        }
    }

    $sorted = $Values | Sort-Object
    $count = $sorted.Count
    $index = [Math]::Ceiling($count * 0.95) - 1
    if ($index -lt 0) { $index = 0 }

    return [PSCustomObject]@{
        Count = $count
        MinMs = [Math]::Round($sorted[0], 2)
        MaxMs = [Math]::Round($sorted[$count - 1], 2)
        AvgMs = [Math]::Round((($sorted | Measure-Object -Average).Average), 2)
        P95Ms = [Math]::Round($sorted[$index], 2)
    }
}

function Invoke-Benchmark {
    param(
        [string]$Name,
        [string]$Uri,
        [int]$ColdRuns,
        [int]$WarmupRuns,
        [int]$HotRuns,
        [int]$TimeoutSec
    )

    Write-Host "\n========================================" -ForegroundColor Cyan
    Write-Host "BENCHMARK: $Name" -ForegroundColor Cyan
    Write-Host "URI: $Uri" -ForegroundColor DarkCyan

    $allRuns = @()

    for ($i = 1; $i -le $ColdRuns; $i++) {
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $null = Invoke-RestMethod -Uri $Uri -Method Get -TimeoutSec $TimeoutSec
        $sw.Stop()

        $allRuns += [PSCustomObject]@{
            Phase = "Cold"
            Run = $i
            DurationMs = [Math]::Round($sw.Elapsed.TotalMilliseconds, 2)
        }
    }

    for ($i = 1; $i -le $WarmupRuns; $i++) {
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $null = Invoke-RestMethod -Uri $Uri -Method Get -TimeoutSec $TimeoutSec
        $sw.Stop()

        $allRuns += [PSCustomObject]@{
            Phase = "Warmup"
            Run = $i
            DurationMs = [Math]::Round($sw.Elapsed.TotalMilliseconds, 2)
        }
    }

    for ($i = 1; $i -le $HotRuns; $i++) {
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $null = Invoke-RestMethod -Uri $Uri -Method Get -TimeoutSec $TimeoutSec
        $sw.Stop()

        $allRuns += [PSCustomObject]@{
            Phase = "Hot"
            Run = $i
            DurationMs = [Math]::Round($sw.Elapsed.TotalMilliseconds, 2)
        }
    }

    $cold = $allRuns | Where-Object { $_.Phase -eq "Cold" } | Select-Object -ExpandProperty DurationMs
    $hot = $allRuns | Where-Object { $_.Phase -eq "Hot" } | Select-Object -ExpandProperty DurationMs

    $coldStat = Get-StatSummary -Values $cold
    $hotStat = Get-StatSummary -Values $hot

    Write-Host "\nCold Summary" -ForegroundColor Yellow
    $coldStat | Format-Table -AutoSize | Out-String | Write-Host

    Write-Host "Hot Summary" -ForegroundColor Yellow
    $hotStat | Format-Table -AutoSize | Out-String | Write-Host

    if ($coldStat.AvgMs -gt 0) {
        $improvement = [Math]::Round((($coldStat.AvgMs - $hotStat.AvgMs) / $coldStat.AvgMs) * 100, 2)
        Write-Host "Estimated improvement (Avg Cold -> Avg Hot): $improvement%" -ForegroundColor Green
    }

    return $allRuns
}

try {
    $entityBaseUri = "$BaseUrl/$Entity"

    Write-Host "Base URL: $BaseUrl" -ForegroundColor DarkGray
    Write-Host "Entity: $Entity" -ForegroundColor DarkGray

    $listResponse = Invoke-RestMethod -Uri $entityBaseUri -Method Get -TimeoutSec $TimeoutSec
    if (-not $listResponse.isSuccess) {
        throw "API list endpoint returned isSuccess=false"
    }

    $items = @($listResponse.data)
    if ($items.Count -eq 0) {
        throw "No data found for entity '$Entity'. Seed data first to run by-id benchmark."
    }

    $idFieldByEntity = @{
        "departments" = "departmentID"
        "employees" = "employeeID"
        "positions" = "positionID"
    }

    $idField = $idFieldByEntity[$Entity]
    $sampleId = $items[0].$idField

    Write-Host "Sample ID: $sampleId" -ForegroundColor DarkGray

    $report = @()
    $report += Invoke-Benchmark -Name "GET /$Entity" -Uri $entityBaseUri -ColdRuns $ColdRuns -WarmupRuns $WarmupRuns -HotRuns $HotRuns -TimeoutSec $TimeoutSec

    if (-not $SkipById) {
        $byIdUri = "$entityBaseUri/$sampleId"
        $report += Invoke-Benchmark -Name "GET /$Entity/{id}" -Uri $byIdUri -ColdRuns $ColdRuns -WarmupRuns $WarmupRuns -HotRuns $HotRuns -TimeoutSec $TimeoutSec
    }

    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $outputFile = "benchmark-result-$Entity-$timestamp.csv"
    $report | Export-Csv -Path $outputFile -NoTypeInformation -Encoding UTF8

    Write-Host "\nSaved detailed runs: $outputFile" -ForegroundColor Cyan
    Write-Host "Done." -ForegroundColor Green
}
catch {
    Write-Host "Benchmark failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
