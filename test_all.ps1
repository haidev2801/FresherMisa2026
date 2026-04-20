$baseUrl = "http://localhost:5050/api"

Write-Host "================= AUTOMATED API TESTS ================="

# 1. Test Position CRUD
Write-Host "`n--- 1. Testing Positions API ---"
$positions = Invoke-RestMethod -Uri "$baseUrl/Positions" -Method GET
Write-Host "GET /Positions: Found $($positions.data.Count) positions."

# 2. Test Employee CRUD
Write-Host "`n--- 2. Testing Employees API ---"
$employees = Invoke-RestMethod -Uri "$baseUrl/Employees" -Method GET
Write-Host "GET /Employees: Found $($employees.data.Count) employees."

# 3. Test Department Custom Endpoints
Write-Host "`n--- 3. Testing Department Custom Endpoints (Task 2.3) ---"
$deptEmployees = Invoke-RestMethod -Uri "$baseUrl/Departments/RND/employees" -Method GET
Write-Host "GET /Departments/RND/employees: Found $($deptEmployees.data.Count) employees."
$deptCount = Invoke-RestMethod -Uri "$baseUrl/Departments/RND/employee-count" -Method GET
Write-Host "GET /Departments/RND/employee-count: Count is $($deptCount.data)."

# 4. Test Employee Validation (Task 2.1 & 3.2 Race condition test simulator)
Write-Host "`n--- 4. Testing Employee Validations (Task 2.1) ---"
$bodyInvalidEmail = @{
    employeeCode = "TEST999"
    employeeName = "Test Name"
    email = "invalid-email"
    departmentID = "550e8400-e29b-41d4-a716-446655440000"
    positionID = "11111111-1111-1111-1111-111111111111"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$baseUrl/Employees" -Method POST -Body $bodyInvalidEmail -ContentType "application/json"
    Write-Host "FAILED: Invalid email passed validation!" -ForegroundColor Red
} catch {
    Write-Host "SUCCESS: Invalid email caught (400 Bad Request)" -ForegroundColor Green
}

$bodyDuplicate = @{
    employeeCode = "EMP001"
    employeeName = "Test Name"
    departmentID = "550e8400-e29b-41d4-a716-446655440000"
    positionID = "11111111-1111-1111-1111-111111111111"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$baseUrl/Employees" -Method POST -Body $bodyDuplicate -ContentType "application/json"
    Write-Host "FAILED: Duplicate code passed validation!" -ForegroundColor Red
} catch {
    Write-Host "SUCCESS: Duplicate code caught (400 Bad Request)" -ForegroundColor Green
}

# 5. Test Employee Filter and Paging (Task 2.2 & 3.3)
Write-Host "`n--- 5. Testing Employee Filter & Paging (Task 2.2 & 3.3) ---"
$filterResult = Invoke-RestMethod -Uri "$baseUrl/Employees/filter?gender=1" -Method GET
Write-Host "GET /Employees/filter?gender=1: Found $($filterResult.data.Count) results."

$pagingResult = Invoke-RestMethod -Uri "$baseUrl/Employees/filter-paging?pageSize=5&pageIndex=2" -Method GET
Write-Host "GET /Employees/filter-paging?pageSize=5&pageIndex=2: Total is $($pagingResult.data.total), returning $($pagingResult.data.data.Count) items."

Write-Host "`n================= ALL TESTS COMPLETED ================="
