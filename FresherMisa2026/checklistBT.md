## Bài Tập Cấp Độ 1: Beginner

### Task 1.1: Hoàn Thiện CRUD cho Position (1 điểm)

**Yêu cầu:**
- Tạo `PositionRepository` kế thừa `BaseRepository<Position>` - DONE
- Tạo `PositionService` kế thừa `BaseService<Position>` - DONE
- Tạo `PositionsController` kế thừa `BaseController<Position>` - DONE
- Đăng ký DI trong `ServiceExtensions.cs` - DONE

**Kiểm tra:**
- [X] API `GET /api/Positions` trả về danh sách position
- [X] API `GET /api/Positions/{id}` trả về position theo id
- [X] API `POST /api/Positions` thêm mới position
    fix lỗi: 
        sai utf dtb:    ALTER TABLE Position
                        CONVERT TO CHARACTER SET utf8mb4
                        COLLATE utf8mb4_0900_ai_ci; 
        Thiếu tạo ID trong repo => luôn lấy id 00000-000.. mặc định
- [X] API `PUT /api/Positions/{id}` cập nhật position
    fix lỗi: setid sau set param nên id không lấy được
- [X] API `DELETE /api/Positions/{id}` xóa position

