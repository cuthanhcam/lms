# 🎓 Learning Management System (LMS)

Hệ thống quản lý học tập trực tuyến hoàn chỉnh với Backend .NET và Frontend React TypeScript.

## 📦 Projects

### 🖥️ Backend - LMS (.NET)
- **Technology**: ASP.NET Core 9, Entity Framework Core, SQL Server
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)
- **Features**: 
  - Authentication & Authorization (JWT)
  - Course Management (CRUD)
  - Lesson Management
  - Student Enrollment
  - Role-based access control

📍 **Location**: `LMS/`

### 🎨 Frontend - LMS.Web (React TypeScript)
- **Technology**: React 18, TypeScript, Vite, Tailwind CSS
- **State Management**: TanStack Query, Zustand
- **Features**:
  - Modern, clean UI design
  - Full CRUD for Courses & Lessons
  - Student enrollment system
  - Role-based UI (Student, Instructor, Admin)
  - Responsive design

📍 **Location**: `LMS.Web/`

## 🚀 Quick Start

### Backend
```bash
cd LMS
dotnet restore
dotnet run --project src/LMS.API
```

### Frontend
```bash
cd LMS.Web
npm install
npm run dev
```

## 📖 Documentation

- [Backend Documentation](./LMS/README.md)
- [Frontend Documentation](./LMS.Web/README.md)
- [Frontend Quick Start](./LMS.Web/QUICKSTART.md)
- [Frontend Structure](./LMS.Web/STRUCTURE.md)

## 🎯 Features

✅ User Authentication & Authorization  
✅ Course Management (Create, Read, Update, Delete)  
✅ Lesson Management  
✅ Student Enrollment System  
✅ Role-based Access Control (Student, Instructor, Admin)  
✅ Modern React TypeScript Frontend  
✅ RESTful API with .NET Backend  
✅ Clean Architecture  
✅ Responsive Design  

## 🏗️ Architecture

```
lms/
├── LMS/              # Backend .NET
│   ├── src/
│   │   ├── LMS.API/              # Web API
│   │   ├── LMS.Application/      # Business Logic
│   │   ├── LMS.Domain/           # Domain Models
│   │   ├── LMS.Infrastructure/   # Data Access
│   │   └── LMS.Shared/           # Shared Resources
│   └── tests/
│
└── LMS.Web/          # Frontend React TypeScript
    ├── src/
    │   ├── api/          # API Integration
    │   ├── components/   # React Components
    │   ├── pages/        # Page Components
    │   ├── layouts/      # Layouts
    │   ├── stores/       # State Management
    │   └── types/        # TypeScript Types
    └── public/

## 👥 User Roles

### 👨‍🎓 Student
- Browse and search courses
- Enroll in courses
- Track learning progress
- View enrolled courses

### 👨‍🏫 Instructor
- Create and manage courses
- Add and edit lessons
- Publish/unpublish courses
- View course statistics

### 👨‍💼 Admin
- All instructor permissions
- Delete any course
- Manage system settings

## 🛠️ Tech Stack

### Backend
- .NET 8
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger/OpenAPI

### Frontend
- React 18
- TypeScript
- Vite
- Tailwind CSS
- TanStack Query
- Zustand
- React Router
- React Hook Form
- Zod

## 📝 License

MIT

---

**Developed with ❤️**
```
