# Task 3.1: Caching BaseRepository

## Vấn đề

Mỗi lần gọi `GET /api/Employees` hay `GET /api/Employees/{id}`, server đều truy vấn database — dù dữ liệu vừa được lấy xong 1 giây trước. Đây là lãng phí không cần thiết.

## Giải pháp

Lưu kết quả vào bộ nhớ RAM (**cache**) trong 5 phút. Lần sau gọi cùng API → trả từ RAM luôn, không cần hỏi DB.

```
Lần 1: API → DB → trả kết quả → lưu vào cache
Lần 2: API → cache hit → trả kết quả (không cần DB)
```

## Cache bị xoá khi nào?

Khi có thay đổi dữ liệu (thêm/sửa/xoá), cache cũ bị xoá ngay để tránh trả về dữ liệu lỗi thời:

| Thao tác | Cache bị xoá |
|----------|-------------|
| Thêm mới | Danh sách |
| Cập nhật | Danh sách + bản ghi đó |
| Xoá | Danh sách + bản ghi đó |

## Tại sao chọn IMemoryCache?

Project chạy 1 server → lưu trực tiếp trong RAM là đủ, đơn giản, không cần cài thêm Redis.
