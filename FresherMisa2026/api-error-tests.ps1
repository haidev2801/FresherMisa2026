# =====================================================
# Error Validation API Tests - FresherMisa2026 (PowerShell)
# Run: .\api-error-tests.ps1
# Base URL: http://localhost:5237
# =====================================================

# URL gốc của API. Có thể đổi port nếu bạn chạy app ở cổng khác.
$BASE_URL = "http://localhost:5237/api"

# Biến đếm kết quả test.
# passed: số case đạt
# failed: số case không đạt
$passed = 0
$failed = 0

# Hàm chạy 1 test case theo kiểu table-driven (truyền tham số là chạy được).
# Mỗi case gồm:
# - Name: ten de doc tren console
# - Method: GET/POST/PUT/DELETE
# - Url: endpoint cần gọi
# - ExpectedStatus: mã HTTP mong đợi
# - Body: payload JSON (nếu có)
# - Contains: chuỗi cần xuất hiện trong response (tùy chọn)
function Invoke-ApiCase {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [int]$ExpectedStatus,
        [string]$Body = "",
        [string]$Contains = ""
    )

    try {
        # Nếu không có body -> gọi API không kèm payload.
        if ([string]::IsNullOrWhiteSpace($Body)) {
            $resp = Invoke-WebRequest -Uri $Url -Method $Method -UseBasicParsing
        }
        else {
            # Có body -> set ContentType JSON để backend parse đúng.
            $resp = Invoke-WebRequest -Uri $Url -Method $Method -ContentType "application/json" -Body $Body -UseBasicParsing
        }

        # Request thành công (2xx)
        $status = [int]$resp.StatusCode
        $content = $resp.Content
    }
    catch {
        # Request trả về 4xx/5xx sẽ vào catch.
        # Vẫn đọc status/content để so sánh với expected.
        $status = [int]$_.Exception.Response.StatusCode
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $content = $reader.ReadToEnd()
    }

    # Rule pass cơ bản: status phải bằng expected.
    $ok = $status -eq $ExpectedStatus

    # Rule pass bổ sung (nếu có): body phải chứa một chuỗi mong đợi.
    # Dùng cho các case cần xác minh thông điệp lỗi cụ thể.
    if ($ok -and -not [string]::IsNullOrWhiteSpace($Contains)) {
        $ok = $content -like "*$Contains*"
    }

    if ($ok) {
        $script:passed++
        Write-Host "[PASS] $Name => $status" -ForegroundColor Green
    }
    else {
        $script:failed++
        Write-Host "[FAIL] $Name => actual=$status expected=$ExpectedStatus" -ForegroundColor Red
        Write-Host "       URL: $Url" -ForegroundColor DarkGray
        Write-Host "       Body: $content" -ForegroundColor DarkGray
    }
}

# =====================================================
# NHÓM 1: POSITION CRUD - CÁC CASE LỖI
# Mục tiêu: đảm bảo API Position trả đúng 400/404 trong các tình huống lỗi phổ biến.
# =====================================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "POSITION CRUD - ERROR VALIDATION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 1) ID sai định dạng GUID -> 400
Invoke-ApiCase -Name "Position GET invalid guid" -Method GET -Url "$BASE_URL/positions/not-a-guid" -ExpectedStatus 400

# 2) ID đúng format nhưng không tồn tại -> 404
Invoke-ApiCase -Name "Position GET not found" -Method GET -Url "$BASE_URL/positions/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" -ExpectedStatus 404

# 3) Tạo mới nhưng body rỗng -> 400 (thiếu field bắt buộc)
Invoke-ApiCase -Name "Position POST empty body" -Method POST -Url "$BASE_URL/positions" -ExpectedStatus 400 -Body "{}"

# 4) Update với ID route sai format GUID -> 400
Invoke-ApiCase -Name "Position PUT invalid guid" -Method PUT -Url "$BASE_URL/positions/not-a-guid" -ExpectedStatus 400 -Body '{"positionID":"00000000-0000-0000-0000-000000000000","positionCode":"P1","positionName":"Name"}'

# 5) Xóa bản ghi không tồn tại -> 404
Invoke-ApiCase -Name "Position DELETE not found" -Method DELETE -Url "$BASE_URL/positions/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" -ExpectedStatus 404

# 6) Tìm theo code không tồn tại -> 404
Invoke-ApiCase -Name "Position GET by code not found" -Method GET -Url "$BASE_URL/positions/Code/NOPE999" -ExpectedStatus 404

# =====================================================
# NHÓM 2: DEPARTMENT CRUD - CÁC CASE LỖI
# Mục tiêu tương tự Position, nhưng áp dụng cho Department.
# =====================================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "DEPARTMENT CRUD - ERROR VALIDATION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 7) ID sai định dạng GUID -> 400
Invoke-ApiCase -Name "Department GET invalid guid" -Method GET -Url "$BASE_URL/departments/not-a-guid" -ExpectedStatus 400

# 8) ID đúng format nhưng không tồn tại -> 404
Invoke-ApiCase -Name "Department GET not found" -Method GET -Url "$BASE_URL/departments/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb" -ExpectedStatus 404

# 9) Tạo mới nhưng body rỗng -> 400
Invoke-ApiCase -Name "Department POST empty body" -Method POST -Url "$BASE_URL/departments" -ExpectedStatus 400 -Body "{}"

# 10) Update với ID route sai format GUID -> 400
Invoke-ApiCase -Name "Department PUT invalid guid" -Method PUT -Url "$BASE_URL/departments/not-a-guid" -ExpectedStatus 400 -Body '{"departmentID":"00000000-0000-0000-0000-000000000000","departmentCode":"D1","departmentName":"Name","description":"Desc"}'

# 11) Xóa bản ghi không tồn tại -> 404
Invoke-ApiCase -Name "Department DELETE not found" -Method DELETE -Url "$BASE_URL/departments/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb" -ExpectedStatus 404

# 12) Tìm theo code không tồn tại -> 404
Invoke-ApiCase -Name "Department GET by code not found" -Method GET -Url "$BASE_URL/departments/Code/NOPE999" -ExpectedStatus 404

# =====================================================
# NHÓM 3: EMPLOYEE CRUD + FILTER VALIDATION
# Ngoài CRUD còn có thêm validate filter nghiệp vụ.
# =====================================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "EMPLOYEE CRUD - ERROR VALIDATION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 13) ID sai định dạng GUID -> 400
Invoke-ApiCase -Name "Employee GET invalid guid" -Method GET -Url "$BASE_URL/employees/not-a-guid" -ExpectedStatus 400

# 14) ID đúng format nhưng không tồn tại -> 404
Invoke-ApiCase -Name "Employee GET not found" -Method GET -Url "$BASE_URL/employees/cccccccc-cccc-cccc-cccc-cccccccccccc" -ExpectedStatus 404

# 15) Tạo mới nhưng body rỗng -> 400
Invoke-ApiCase -Name "Employee POST empty body" -Method POST -Url "$BASE_URL/employees" -ExpectedStatus 400 -Body "{}"

# 16) Tạo mới với email/phone sai format -> 400
Invoke-ApiCase -Name "Employee POST bad email/phone" -Method POST -Url "$BASE_URL/employees" -ExpectedStatus 400 -Body '{"employeeID":"55555555-5555-5555-5555-555555555555","employeeCode":"EMP_ERR_01","employeeName":"Err User","departmentID":"11111111-1111-1111-1111-111111111111","positionID":"22222222-2222-2222-2222-222222222222","email":"bad-email","phoneNumber":"abc"}'

# 17) Update với ID route sai format GUID -> 400
Invoke-ApiCase -Name "Employee PUT invalid guid" -Method PUT -Url "$BASE_URL/employees/not-a-guid" -ExpectedStatus 400 -Body '{"employeeID":"55555555-5555-5555-5555-555555555555","employeeCode":"EMP_ERR_01","employeeName":"Err User","departmentID":"11111111-1111-1111-1111-111111111111","positionID":"22222222-2222-2222-2222-222222222222"}'

# 18) Xóa bản ghi không tồn tại -> 404
Invoke-ApiCase -Name "Employee DELETE not found" -Method DELETE -Url "$BASE_URL/employees/cccccccc-cccc-cccc-cccc-cccccccccccc" -ExpectedStatus 404

# 19) Tìm theo code không tồn tại -> 404
Invoke-ApiCase -Name "Employee GET by code not found" -Method GET -Url "$BASE_URL/employees/Code/NOPE999" -ExpectedStatus 404

# 20) Filter gender ngoài phạm vi hợp lệ (0,1,2) -> 400
Invoke-ApiCase -Name "Employee filter invalid gender" -Method GET -Url "$BASE_URL/employees/filter?gender=99" -ExpectedStatus 400

# 21) Filter salary range sai (from > to) -> 400
Invoke-ApiCase -Name "Employee filter invalid salary range" -Method GET -Url "$BASE_URL/employees/filter?salaryFrom=1000&salaryTo=100" -ExpectedStatus 400

# 22) Filter hire date range sai (from > to) -> 400
Invoke-ApiCase -Name "Employee filter invalid hire date range" -Method GET -Url "$BASE_URL/employees/filter?hireDateFrom=2026-05-01&hireDateTo=2026-04-01" -ExpectedStatus 400

# =====================================================
# TỔNG KẾT
# Nếu còn case fail -> exit code 1 
# Nếu pass hết -> exit code 0.
# =====================================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "ERROR VALIDATION SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor Yellow

if ($failed -gt 0) {
    exit 1
}

exit 0
