# =====================================================
# Full API Regression Tests - FresherMisa2026
# Run: .\api-error-tests.ps1
# Base URL: http://localhost:5237
# =====================================================

$BASE_URL = "http://localhost:5237/api"
$passed = 0
$failed = 0

function Convert-ToSafeJson {
    param([string]$Content)
    if ([string]::IsNullOrWhiteSpace($Content)) { return $null }
    try { return $Content | ConvertFrom-Json } catch { return $null }
}

function New-JsonBody {
    param([hashtable]$Data)
    return ($Data | ConvertTo-Json -Depth 20 -Compress)
}

function Invoke-ApiCase {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [int]$ExpectedStatus,
        [string]$Body = "",
        [string]$Contains = "",
        [scriptblock]$Assert = $null
    )

    $status = -1
    $content = ""

    try {
        if ([string]::IsNullOrWhiteSpace($Body)) {
            $resp = Invoke-WebRequest -Uri $Url -Method $Method -UseBasicParsing
        }
        else {
            $resp = Invoke-WebRequest -Uri $Url -Method $Method -ContentType "application/json" -Body $Body -UseBasicParsing
        }

        $status = [int]$resp.StatusCode
        $content = $resp.Content
    }
    catch {
        if ($_.Exception.Response -ne $null) {
            $status = [int]$_.Exception.Response.StatusCode
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $content = $reader.ReadToEnd()
            $reader.Dispose()
        }
        else {
            $status = 0
            $content = $_.Exception.Message
        }
    }

    $json = Convert-ToSafeJson -Content $content
    $ok = $status -eq $ExpectedStatus

    if ($ok -and -not [string]::IsNullOrWhiteSpace($Contains)) {
        $ok = $content -like "*$Contains*"
    }

    if ($ok -and $Assert -ne $null) {
        try { $ok = & $Assert $status $content $json }
        catch { $ok = $false }
    }

    if ($ok) {
        $script:passed++
        Write-Host "[PASS] $Name => $status" -ForegroundColor Green
    }
    else {
        $script:failed++
        Write-Host "[FAIL] $Name => actual=$status expected=$ExpectedStatus" -ForegroundColor Red
        Write-Host "       URL: $Url" -ForegroundColor DarkGray
        if (-not [string]::IsNullOrWhiteSpace($Body)) {
            Write-Host "       Request Body: $Body" -ForegroundColor DarkGray
        }
        Write-Host "       Response Body: $content" -ForegroundColor DarkGray
    }

    return [pscustomobject]@{
        Name = $Name
        Status = $status
        Content = $content
        Json = $json
        IsPassed = $ok
    }
}

# =====================================================
# Shared test data
# Keep codes <= 20 chars due validation rule
# =====================================================
$stamp = Get-Date -Format "yyMMddHHmmss"

$positionId = [guid]::NewGuid().ToString()
$departmentId = [guid]::NewGuid().ToString()
$employeeId = [guid]::NewGuid().ToString()

$positionCode = "P$stamp"
$departmentCode = "D$stamp"
$employeeCode = "E$stamp"

$positionCodeUpdated = "PU$stamp"
$departmentCodeUpdated = "DU$stamp"
$employeeCodeUpdated = "EU$stamp"

$positionPayload = New-JsonBody @{
    positionID = $positionId
    positionCode = $positionCode
    positionName = "Position Test"
}

$departmentPayload = New-JsonBody @{
    departmentID = $departmentId
    departmentCode = $departmentCode
    departmentName = "Department Test"
    description = "Department test"
}

$employeePayload = New-JsonBody @{
    employeeID = $employeeId
    employeeCode = $employeeCode
    employeeName = "Employee Test"
    gender = 1
    dateOfBirth = "2000-01-01"
    phoneNumber = "0912345678"
    email = "e$stamp@test.local"
    address = "Hanoi"
    departmentID = $departmentId
    positionID = $positionId
    salary = 15000000
    hireDate = "2024-01-01"
}

$positionUpdatePayload = New-JsonBody @{
    positionID = $positionId
    positionCode = $positionCodeUpdated
    positionName = "Position Updated"
}

$departmentUpdatePayload = New-JsonBody @{
    departmentID = $departmentId
    departmentCode = $departmentCodeUpdated
    departmentName = "Department Updated"
    description = "Department updated"
}

$employeeUpdatePayload = New-JsonBody @{
    employeeID = $employeeId
    employeeCode = $employeeCodeUpdated
    employeeName = "Employee Updated"
    gender = 0
    dateOfBirth = "1999-12-31"
    phoneNumber = "0987654321"
    email = "u$stamp@test.local"
    address = "Da Nang"
    departmentID = $departmentId
    positionID = $positionId
    salary = 18000000
    hireDate = "2024-02-01"
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "GROUP 1 - SMOKE READ APIS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Invoke-ApiCase -Name "Departments GET list" -Method GET -Url "$BASE_URL/departments" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Positions GET list" -Method GET -Url "$BASE_URL/positions" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Employees GET list" -Method GET -Url "$BASE_URL/employees" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Departments GET paging" -Method GET -Url "$BASE_URL/departments/paging?pageSize=5`&pageIndex=1" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Positions GET paging" -Method GET -Url "$BASE_URL/positions/paging?pageSize=5`&pageIndex=1" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Employees GET paging" -Method GET -Url "$BASE_URL/employees/paging?pageSize=5`&pageIndex=1" -ExpectedStatus 200 | Out-Null

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "GROUP 2 - CRUD SUCCESS FLOW" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Invoke-ApiCase -Name "Position POST create" -Method POST -Url "$BASE_URL/positions" -ExpectedStatus 201 -Body $positionPayload | Out-Null
Invoke-ApiCase -Name "Department POST create" -Method POST -Url "$BASE_URL/departments" -ExpectedStatus 201 -Body $departmentPayload | Out-Null
Invoke-ApiCase -Name "Employee POST create" -Method POST -Url "$BASE_URL/employees" -ExpectedStatus 201 -Body $employeePayload | Out-Null

Invoke-ApiCase -Name "Position GET by id" -Method GET -Url "$BASE_URL/positions/$positionId" -ExpectedStatus 200 -Contains $positionCode | Out-Null
Invoke-ApiCase -Name "Department GET by id" -Method GET -Url "$BASE_URL/departments/$departmentId" -ExpectedStatus 200 -Contains $departmentCode | Out-Null
Invoke-ApiCase -Name "Employee GET by id" -Method GET -Url "$BASE_URL/employees/$employeeId" -ExpectedStatus 200 -Contains $employeeCode | Out-Null

Invoke-ApiCase -Name "Position GET by code" -Method GET -Url "$BASE_URL/positions/code/$positionCode" -ExpectedStatus 200 -Contains $positionCode | Out-Null
Invoke-ApiCase -Name "Department GET by code" -Method GET -Url "$BASE_URL/departments/code/$departmentCode" -ExpectedStatus 200 -Contains $departmentCode | Out-Null
Invoke-ApiCase -Name "Employee GET by code" -Method GET -Url "$BASE_URL/employees/code/$employeeCode" -ExpectedStatus 200 -Contains $employeeCode | Out-Null

Invoke-ApiCase -Name "Employees GET by departmentId" -Method GET -Url "$BASE_URL/employees/department/$departmentId" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Employees GET by positionId" -Method GET -Url "$BASE_URL/employees/position/$positionId" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Departments GET employees by code" -Method GET -Url "$BASE_URL/departments/$departmentCode/employees" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Departments GET employee count by code" -Method GET -Url "$BASE_URL/departments/$departmentCode/employee-count" -ExpectedStatus 200 -Assert {
    param($status, $content, $json)
    if ($null -eq $json) { return $false }

    $countValue = $null
    if ($null -ne $json.Data) {
        $countValue = $json.Data
    }
    elseif ($null -ne $json.data) {
        $countValue = $json.data
    }

    if ($null -eq $countValue) { return $false }
    return [int64]$countValue -ge 1
} | Out-Null

Invoke-ApiCase -Name "Employees filter success" -Method GET -Url "$BASE_URL/employees/filter?pageSize=10`&pageIndex=1`&gender=1" -ExpectedStatus 200 | Out-Null

Invoke-ApiCase -Name "Position PUT update" -Method PUT -Url "$BASE_URL/positions/$positionId" -ExpectedStatus 200 -Body $positionUpdatePayload | Out-Null
Invoke-ApiCase -Name "Department PUT update" -Method PUT -Url "$BASE_URL/departments/$departmentId" -ExpectedStatus 200 -Body $departmentUpdatePayload | Out-Null
Invoke-ApiCase -Name "Employee PUT update" -Method PUT -Url "$BASE_URL/employees/$employeeId" -ExpectedStatus 200 -Body $employeeUpdatePayload | Out-Null

Invoke-ApiCase -Name "Position verify updated" -Method GET -Url "$BASE_URL/positions/$positionId" -ExpectedStatus 200 -Contains $positionCodeUpdated | Out-Null
Invoke-ApiCase -Name "Department verify updated" -Method GET -Url "$BASE_URL/departments/$departmentId" -ExpectedStatus 200 -Contains $departmentCodeUpdated | Out-Null
Invoke-ApiCase -Name "Employee verify updated" -Method GET -Url "$BASE_URL/employees/$employeeId" -ExpectedStatus 200 -Contains $employeeCodeUpdated | Out-Null

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "GROUP 3 - VALIDATION AND ERROR CASES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Invoke-ApiCase -Name "Position GET invalid guid" -Method GET -Url "$BASE_URL/positions/not-a-guid" -ExpectedStatus 400 | Out-Null
Invoke-ApiCase -Name "Department GET invalid guid" -Method GET -Url "$BASE_URL/departments/not-a-guid" -ExpectedStatus 400 | Out-Null
Invoke-ApiCase -Name "Employee GET invalid guid" -Method GET -Url "$BASE_URL/employees/not-a-guid" -ExpectedStatus 400 | Out-Null

Invoke-ApiCase -Name "Position PUT invalid guid" -Method PUT -Url "$BASE_URL/positions/not-a-guid" -ExpectedStatus 400 -Body $positionPayload | Out-Null
Invoke-ApiCase -Name "Department PUT invalid guid" -Method PUT -Url "$BASE_URL/departments/not-a-guid" -ExpectedStatus 400 -Body $departmentPayload | Out-Null
Invoke-ApiCase -Name "Employee PUT invalid guid" -Method PUT -Url "$BASE_URL/employees/not-a-guid" -ExpectedStatus 400 -Body $employeePayload | Out-Null

Invoke-ApiCase -Name "Position POST empty body" -Method POST -Url "$BASE_URL/positions" -ExpectedStatus 400 -Body "{}" | Out-Null
Invoke-ApiCase -Name "Department POST empty body" -Method POST -Url "$BASE_URL/departments" -ExpectedStatus 400 -Body "{}" | Out-Null
Invoke-ApiCase -Name "Employee POST empty body" -Method POST -Url "$BASE_URL/employees" -ExpectedStatus 400 -Body "{}" | Out-Null

Invoke-ApiCase -Name "Employee POST bad email and phone" -Method POST -Url "$BASE_URL/employees" -ExpectedStatus 400 -Body '{"employeeID":"55555555-5555-5555-5555-555555555555","employeeCode":"EERR01","employeeName":"Err User","departmentID":"11111111-1111-1111-1111-111111111111","positionID":"22222222-2222-2222-2222-222222222222","email":"bad-email","phoneNumber":"abc"}' | Out-Null

Invoke-ApiCase -Name "Position GET by code not found" -Method GET -Url "$BASE_URL/positions/code/NOPE999" -ExpectedStatus 404 | Out-Null
Invoke-ApiCase -Name "Department GET by code not found" -Method GET -Url "$BASE_URL/departments/code/NOPE999" -ExpectedStatus 404 | Out-Null
Invoke-ApiCase -Name "Employee GET by code not found" -Method GET -Url "$BASE_URL/employees/code/NOPE999" -ExpectedStatus 404 | Out-Null

Invoke-ApiCase -Name "Department GET employees by code not found" -Method GET -Url "$BASE_URL/departments/NOPE999/employees" -ExpectedStatus 404 | Out-Null
Invoke-ApiCase -Name "Department GET employee count by code not found" -Method GET -Url "$BASE_URL/departments/NOPE999/employee-count" -ExpectedStatus 404 | Out-Null

Invoke-ApiCase -Name "Employee filter invalid gender" -Method GET -Url "$BASE_URL/employees/filter?gender=99" -ExpectedStatus 400 | Out-Null
Invoke-ApiCase -Name "Employee filter invalid salary range" -Method GET -Url "$BASE_URL/employees/filter?salaryFrom=1000`&salaryTo=100" -ExpectedStatus 400 | Out-Null
Invoke-ApiCase -Name "Employee filter invalid hire date range" -Method GET -Url "$BASE_URL/employees/filter?hireDateFrom=2026-05-01`&hireDateTo=2026-04-01" -ExpectedStatus 400 | Out-Null

Invoke-ApiCase -Name "Department DELETE while has employee" -Method DELETE -Url "$BASE_URL/departments/$departmentId" -ExpectedStatus 400 | Out-Null

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "GROUP 4 - CLEANUP AND VERIFY DELETE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Invoke-ApiCase -Name "Employee DELETE success" -Method DELETE -Url "$BASE_URL/employees/$employeeId" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Department DELETE success" -Method DELETE -Url "$BASE_URL/departments/$departmentId" -ExpectedStatus 200 | Out-Null
Invoke-ApiCase -Name "Position DELETE success" -Method DELETE -Url "$BASE_URL/positions/$positionId" -ExpectedStatus 200 | Out-Null

Invoke-ApiCase -Name "Employee GET after delete" -Method GET -Url "$BASE_URL/employees/$employeeId" -ExpectedStatus 404 | Out-Null
Invoke-ApiCase -Name "Department GET after delete" -Method GET -Url "$BASE_URL/departments/$departmentId" -ExpectedStatus 404 | Out-Null
Invoke-ApiCase -Name "Position GET after delete" -Method GET -Url "$BASE_URL/positions/$positionId" -ExpectedStatus 404 | Out-Null

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "API REGRESSION SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor Yellow

if ($failed -gt 0) {
    exit 1
}

exit 0
