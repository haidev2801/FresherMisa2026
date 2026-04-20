# =====================================================
# API Test Scripts - FresherMisa2026 (PowerShell)
# Run: .\api-tests.ps1
# Base URL: must match FresherMisa2026.WebAPI Properties/launchSettings.json (default http://localhost:5237)
# =====================================================

param(
    [string]$BaseUrl = "http://localhost:5237/api"
)
$BASE_URL = $BaseUrl.TrimEnd('/')

# Seed IDs from DB / ChangeLogs (must exist for FK & lookup tests)
$SAMPLE_DEPT_ID = "550e8400-e29b-41d4-a716-446655440010"   # RND
$SAMPLE_DEPT_CODE = "RND"
$SAMPLE_DEPT_WITH_EMPLOYEES = "550e8400-e29b-41d4-a716-446655440002"
$SAMPLE_POSITION_ID = "11111111-1111-1111-1111-111111111111" # DEV
$SAMPLE_POSITION_CODE = "DEV"
$SAMPLE_EMPLOYEE_CODE = "EMP001"
$SAMPLE_EMPLOYEE_ID = "e0000001-0000-0000-0000-000000000001"

function Show-Json {
    param($Object)
    $Object | ConvertTo-Json -Depth 10
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "DEPARTMENT API TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 1. Get all departments
Write-Host "`n--- 1. Get all departments ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments" -Method Get)

# 2. Get department by ID (empty GUID is rejected by API by design)
Write-Host "`n--- 2. Get department by ID ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments/$SAMPLE_DEPT_ID" -Method Get)

# 3. Get departments with paging
Write-Host "`n--- 3. Get departments with paging ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments/Paging?pageSize=10&pageIndex=1" -Method Get)

# 4. Search departments
Write-Host "`n--- 4. Search departments ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments/Paging?search=phong&pageSize=10&pageIndex=1" -Method Get)

# 5. Sort departments
Write-Host "`n--- 5. Sort departments ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments/Paging?sort=-DepartmentName&pageSize=10&pageIndex=1" -Method Get)

# 6. Get department by code
Write-Host "`n--- 6. Get department by code ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments/Code/$SAMPLE_DEPT_CODE" -Method Get)

# 7–9 CRUD: use new GUIDs so we never collide with seed PKs (e.g. 2222… is QA in DB)
$crudDeptId = [guid]::NewGuid().ToString()
$crudDeptCode = "T{0}" -f ([guid]::NewGuid().ToString("N").Substring(0, 7))

# 7. Create new department
Write-Host "`n--- 7. Create new department ---" -ForegroundColor Yellow
$body = @{
    departmentID   = $crudDeptId
    departmentCode = $crudDeptCode
    departmentName = "Phong Test"
    description    = "Mo ta test"
} | ConvertTo-Json

Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments" -Method Post -Body $body -ContentType "application/json")

# 8. Update department
Write-Host "`n--- 8. Update department ---" -ForegroundColor Yellow
$body = @{
    departmentID   = $crudDeptId
    departmentCode = $crudDeptCode
    departmentName = "Phong Test Updated"
    description    = "Mo ta updated"
} | ConvertTo-Json

Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments/$crudDeptId" -Method Put -Body $body -ContentType "application/json")

# 9. Delete department
Write-Host "`n--- 9. Delete department ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/departments/$crudDeptId" -Method Delete)

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "POSITION API TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 10. Get all positions
Write-Host "`n--- 10. Get all positions ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/positions" -Method Get)

# 11. Get position by ID
Write-Host "`n--- 11. Get position by ID ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/positions/$SAMPLE_POSITION_ID" -Method Get)

# 12. Get positions with paging
Write-Host "`n--- 12. Get positions with paging ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/positions/Paging?pageSize=10&pageIndex=1" -Method Get)

# 13. Search positions
Write-Host "`n--- 13. Search positions ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/positions/Paging?search=dev&pageSize=10&pageIndex=1" -Method Get)

# 14. Get position by code
Write-Host "`n--- 14. Get position by code ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/positions/Code/$SAMPLE_POSITION_CODE" -Method Get)

$crudPosId = [guid]::NewGuid().ToString()
$crudPosCode = "P{0}" -f ([guid]::NewGuid().ToString("N").Substring(0, 7))

# 15. Create new position
Write-Host "`n--- 15. Create new position ---" -ForegroundColor Yellow
$body = @{
    positionID   = $crudPosId
    positionCode = $crudPosCode
    positionName = "Vi tri Test"
} | ConvertTo-Json

Show-Json (Invoke-RestMethod -Uri "$BASE_URL/positions" -Method Post -Body $body -ContentType "application/json")

# 16. Update position
Write-Host "`n--- 16. Update position ---" -ForegroundColor Yellow
$body = @{
    positionID   = $crudPosId
    positionCode = $crudPosCode
    positionName = "Vi tri Test Updated"
} | ConvertTo-Json

Show-Json (Invoke-RestMethod -Uri "$BASE_URL/positions/$crudPosId" -Method Put -Body $body -ContentType "application/json")

# 17. Delete position
Write-Host "`n--- 17. Delete position ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/positions/$crudPosId" -Method Delete)

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "EMPLOYEE API TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 18. Get all employees
Write-Host "`n--- 18. Get all employees ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees" -Method Get)

# 19. Get employee by ID
Write-Host "`n--- 19. Get employee by ID ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/$SAMPLE_EMPLOYEE_ID" -Method Get)

# 20. Get employees with paging
Write-Host "`n--- 20. Get employees with paging ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/Paging?pageSize=10&pageIndex=1" -Method Get)

# 21. Search employees
Write-Host "`n--- 21. Search employees ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/Paging?search=nguyen&pageSize=10&pageIndex=1" -Method Get)

# 22. Sort employees
Write-Host "`n--- 22. Sort employees ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/Paging?sort=-EmployeeName&pageSize=10&pageIndex=1" -Method Get)

# 23. Get employee by code
Write-Host "`n--- 23. Get employee by code ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/Code/$SAMPLE_EMPLOYEE_CODE" -Method Get)

# 24. Get employees by department ID
Write-Host "`n--- 24. Get employees by department ID ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/Department/$SAMPLE_DEPT_WITH_EMPLOYEES" -Method Get)

# 25. Get employees by position ID
Write-Host "`n--- 25. Get employees by position ID ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/Position/$SAMPLE_POSITION_ID" -Method Get)

$crudEmpId = [guid]::NewGuid().ToString()
$crudEmpCode = "E{0}" -f ([guid]::NewGuid().ToString("N").Substring(0, 8))

# 26. Create new employee (HireDate is [IRequired] on entity; FKs must exist)
Write-Host "`n--- 26. Create new employee ---" -ForegroundColor Yellow
$body = @{
    employeeID     = $crudEmpId
    employeeCode   = $crudEmpCode
    employeeName   = "Nguyen Van Test"
    gender         = 1
    dateOfBirth    = "1995-01-01"
    phoneNumber    = "0912345678"
    email          = "test_nv@example.com"
    address        = "Ha Noi"
    departmentID   = $SAMPLE_DEPT_WITH_EMPLOYEES
    positionID     = $SAMPLE_POSITION_ID
    salary         = 10000000
    hireDate       = "2026-04-15T00:00:00"
    createdDate    = "2026-04-15T00:00:00"
} | ConvertTo-Json

Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees" -Method Post -Body $body -ContentType "application/json")

# 27. Update employee
Write-Host "`n--- 27. Update employee ---" -ForegroundColor Yellow
$body = @{
    employeeID     = $crudEmpId
    employeeCode   = $crudEmpCode
    employeeName   = "Nguyen Van Test Updated"
    gender         = 1
    dateOfBirth    = "1995-01-01"
    phoneNumber    = "0987654321"
    email          = "test_nv_upd@example.com"
    address        = "TP Ho Chi Minh"
    departmentID   = $SAMPLE_DEPT_WITH_EMPLOYEES
    positionID     = $SAMPLE_POSITION_ID
    salary         = 15000000
    hireDate       = "2026-04-16T00:00:00"
    createdDate    = "2026-04-15T00:00:00"
} | ConvertTo-Json

Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/$crudEmpId" -Method Put -Body $body -ContentType "application/json")

# 28. Delete employee
Write-Host "`n--- 28. Delete employee ---" -ForegroundColor Yellow
Show-Json (Invoke-RestMethod -Uri "$BASE_URL/employees/$crudEmpId" -Method Delete)

# 29. Task 3.2: parallel POST same EmployeeCode (needs UK index on DB + valid dept/pos IDs)
Write-Host "`n--- 29. Task 3.2 Race: 2 parallel POST same EmployeeCode ---" -ForegroundColor Yellow
$raceCode = ('R' + [guid]::NewGuid().ToString('N').Substring(0, 18))
$raceDept = $SAMPLE_DEPT_WITH_EMPLOYEES
$racePos = $SAMPLE_POSITION_ID
$raceApi = $BASE_URL
$raceBlock = {
    param($ApiRoot, $Code, $EmpGuid, $DeptId, $PosId)
    $payload = @{
        employeeID     = $EmpGuid
        employeeCode   = $Code
        employeeName   = 'Race Concurrent'
        gender         = 1
        dateOfBirth    = '1992-06-06'
        phoneNumber    = '0922222222'
        email          = 'race_concurrent@test.com'
        address        = 'HN'
        departmentID   = $DeptId
        positionID     = $PosId
        salary         = 1000
        hireDate       = '2026-04-18T00:00:00'
        createdDate    = '2026-04-18T12:00:00'
    } | ConvertTo-Json
    try {
        $resp = Invoke-WebRequest -Uri "$ApiRoot/employees" -Method Post -Body $payload -ContentType 'application/json' -UseBasicParsing
        return [pscustomobject]@{ Ok = $true; StatusCode = [int]$resp.StatusCode; Text = $resp.Content }
    }
    catch {
        $r = $_.Exception.Response
        if ($null -ne $r) {
            $sr = New-Object System.IO.StreamReader($r.GetResponseStream())
            $txt = $sr.ReadToEnd()
            return [pscustomobject]@{ Ok = $false; StatusCode = [int]$r.StatusCode; Text = $txt }
        }
        return [pscustomobject]@{ Ok = $false; StatusCode = 0; Text = $_.Exception.Message }
    }
}
$gA = [guid]::NewGuid().ToString()
$gB = [guid]::NewGuid().ToString()
$jobA = Start-Job -ScriptBlock $raceBlock -ArgumentList $raceApi, $raceCode, $gA, $raceDept, $racePos
$jobB = Start-Job -ScriptBlock $raceBlock -ArgumentList $raceApi, $raceCode, $gB, $raceDept, $racePos
Wait-Job -Job $jobA, $jobB | Out-Null
$outA = Receive-Job -Job $jobA
$outB = Receive-Job -Job $jobB
Remove-Job -Job $jobA, $jobB -Force -ErrorAction SilentlyContinue
Write-Host "Job A:" $outA.StatusCode $outA.Text
Write-Host "Job B:" $outB.StatusCode $outB.Text
$okCount = ($outA, $outB | Where-Object { $_.Ok }).Count
$hasMsg = (($outA.Text + $outB.Text) -match 'Duplicate employee code')
if ($okCount -eq 1 -and $hasMsg) {
    Write-Host "PASS Task 3.2: exactly one success and duplicate message present." -ForegroundColor Green
}
else {
    Write-Host "CHECK Task 3.2: expected 1 success (201) + 1 bad request with message (verify UK index on DB and dept/pos IDs)." -ForegroundColor Yellow
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "ALL TESTS COMPLETED" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan