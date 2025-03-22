# Learning Management System (LMS) - Project Summary

## Các Entity chính

### 1.1 Bảng Category (Danh mục khóa học)
- **Id** (int, khóa chính)
- **Name** (string, tên danh mục)
- **Description** (string, mô tả)

### 1.2 Bảng Course (Khóa học)
- **Id** (int, khóa chính)
- **Title** (string, tên khóa học)
- **Description** (string, mô tả khóa học)
- **Price** (decimal, giá khóa học)
- **CategoryId** (int, khóa ngoại liên kết với Category)
- **TeacherId** (int, khóa ngoại liên kết với Teacher)
- **Status** (bool, công khai hay không)

### 1.3 Bảng Teacher (Giảng viên)
- **Id** (int, khóa chính)
- **Name** (string, tên giảng viên)
- **Email** (string, email)
- **PasswordHash** (string, mật khẩu mã hóa)
- **Courses** (List<Course>, danh sách khóa học do giảng viên tạo)

### 1.4 Bảng Student (Học viên)
- **Id** (int, khóa chính)
- **Name** (string, tên học viên)
- **Email** (string, email)
- **PasswordHash** (string, mật khẩu mã hóa)
- **Cart** (List<CartItem>, danh sách khóa học trong giỏ hàng)
- **EnrolledCourses** (List<StudentCourse>, danh sách khóa học đã mua)

### 1.5 Bảng CartItem (Giỏ hàng)
- **Id** (int, khóa chính)
- **StudentId** (int, khóa ngoại liên kết Student)
- **CourseId** (int, khóa ngoại liên kết Course)

### 1.6 Bảng StudentCourse (Khóa học đã mua)
- **Id** (int, khóa chính)
- **StudentId** (int, khóa ngoại liên kết Student)
- **CourseId** (int, khóa ngoại liên kết Course)
- **EnrollDate** (DateTime, ngày đăng ký)

---

## Quyền theo Role

### Admin:
- Toàn quyền quản lý (Danh mục, khóa học, giảng viên, học viên).

### Teacher:
- Chỉ có thể thêm/sửa/xóa khóa học do chính họ tạo.

### Student:
- Xem danh sách khóa học.
- Xem chi tiết khóa học.
- Thêm khóa học vào giỏ hàng.
- Thanh toán và xem danh sách khóa học đã mua.

