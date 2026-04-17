# Checklist tiến độ công việc

## ✅ Đã hoàn thành

- [x] **BaseRepository - Insert (Create):**
  - [x] Thêm logic ánh xạ giá trị ID sau khi sinh `entityId`:
    - `var keyName = _modelType.GetKeyName();`
    - `entity.GetType().GetProperty(keyName).SetValue(entity, entityId);`

- [x] **BaseRepository - Update:**
  - [x] Đổi thứ tự giữa bước 1 và bước 2 theo logic mới.

- [x] **MySQL - Đồng bộ charset/collation:**
  - [x] Tắt tạm kiểm tra khóa ngoại: `SET FOREIGN_KEY_CHECKS = 0;`
  - [x] `ALTER TABLE department CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;`
  - [x] `ALTER TABLE position CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;`
  - [x] `ALTER TABLE employee CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;`
  - [x] Bật lại kiểm tra khóa ngoại: `SET FOREIGN_KEY_CHECKS = 1;`

---
