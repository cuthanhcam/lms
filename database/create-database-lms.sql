/*
	LMS database bootstrap script
	Target DB name follows LMS.API appsettings.Development.json
*/

SET NOCOUNT ON;

IF DB_ID(N'LearningManagementSystemDb_Dev') IS NULL
BEGIN
	CREATE DATABASE [LearningManagementSystemDb_Dev];
END;
GO

USE [LearningManagementSystemDb_Dev];
GO

/* ==================== USERS ==================== */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Users
	(
		Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
		UserName NVARCHAR(100) NOT NULL,
		Email NVARCHAR(255) NOT NULL,
		PasswordHash NVARCHAR(500) NOT NULL,
		Role INT NOT NULL CONSTRAINT DF_Users_Role DEFAULT (0),
		IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
		CreatedAt DATETIME2 NOT NULL,

		CONSTRAINT CK_Users_Role CHECK (Role IN (0, 1, 2))
	);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Email' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
	CREATE UNIQUE INDEX IX_Users_Email ON dbo.Users (Email);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_UserName' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
	CREATE INDEX IX_Users_UserName ON dbo.Users (UserName);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Role' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
	CREATE INDEX IX_Users_Role ON dbo.Users (Role);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_IsActive' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
	CREATE INDEX IX_Users_IsActive ON dbo.Users (IsActive);
END;
GO

/* ==================== COURSES ==================== */
IF OBJECT_ID(N'dbo.Courses', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Courses
	(
		Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Courses PRIMARY KEY,
		Title NVARCHAR(200) NOT NULL,
		Description NVARCHAR(2000) NULL,
		Price DECIMAL(18, 2) NOT NULL,
		Currency NVARCHAR(3) NOT NULL CONSTRAINT DF_Courses_Currency DEFAULT (N'USD'),
		CreatedBy UNIQUEIDENTIFIER NOT NULL,
		IsPublished BIT NOT NULL CONSTRAINT DF_Courses_IsPublished DEFAULT (0),
		IsDeleted BIT NOT NULL CONSTRAINT DF_Courses_IsDeleted DEFAULT (0),
		CreatedAt DATETIME2 NOT NULL,

		CONSTRAINT FK_Courses_Users_CreatedBy FOREIGN KEY (CreatedBy)
			REFERENCES dbo.Users (Id) ON DELETE NO ACTION,
		CONSTRAINT CK_Courses_Price CHECK (Price >= 0)
	);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Courses_CreatedBy' AND object_id = OBJECT_ID(N'dbo.Courses'))
BEGIN
	CREATE INDEX IX_Courses_CreatedBy ON dbo.Courses (CreatedBy);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Courses_IsPublished' AND object_id = OBJECT_ID(N'dbo.Courses'))
BEGIN
	CREATE INDEX IX_Courses_IsPublished ON dbo.Courses (IsPublished);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Courses_IsDeleted' AND object_id = OBJECT_ID(N'dbo.Courses'))
BEGIN
	CREATE INDEX IX_Courses_IsDeleted ON dbo.Courses (IsDeleted);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Courses_CreatedAt' AND object_id = OBJECT_ID(N'dbo.Courses'))
BEGIN
	CREATE INDEX IX_Courses_CreatedAt ON dbo.Courses (CreatedAt);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Courses_IsPublished_IsDeleted' AND object_id = OBJECT_ID(N'dbo.Courses'))
BEGIN
	CREATE INDEX IX_Courses_IsPublished_IsDeleted ON dbo.Courses (IsPublished, IsDeleted);
END;
GO

/* ==================== LESSONS ==================== */
IF OBJECT_ID(N'dbo.Lessons', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Lessons
	(
		Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Lessons PRIMARY KEY,
		CourseId UNIQUEIDENTIFIER NOT NULL,
		Title NVARCHAR(200) NOT NULL,
		Content NVARCHAR(10000) NOT NULL,
		[Order] INT NOT NULL,
		IsDeleted BIT NOT NULL CONSTRAINT DF_Lessons_IsDeleted DEFAULT (0),

		CONSTRAINT FK_Lessons_Courses_CourseId FOREIGN KEY (CourseId)
			REFERENCES dbo.Courses (Id) ON DELETE CASCADE,
		CONSTRAINT CK_Lessons_Order CHECK ([Order] > 0)
	);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Lessons_CourseId_Order' AND object_id = OBJECT_ID(N'dbo.Lessons'))
BEGIN
	CREATE UNIQUE INDEX IX_Lessons_CourseId_Order ON dbo.Lessons (CourseId, [Order]);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Lessons_CourseId' AND object_id = OBJECT_ID(N'dbo.Lessons'))
BEGIN
	CREATE INDEX IX_Lessons_CourseId ON dbo.Lessons (CourseId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Lessons_IsDeleted' AND object_id = OBJECT_ID(N'dbo.Lessons'))
BEGIN
	CREATE INDEX IX_Lessons_IsDeleted ON dbo.Lessons (IsDeleted);
END;
GO

/* ==================== ENROLLMENTS ==================== */
IF OBJECT_ID(N'dbo.Enrollments', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Enrollments
	(
		Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Enrollments PRIMARY KEY,
		UserId UNIQUEIDENTIFIER NOT NULL,
		CourseId UNIQUEIDENTIFIER NOT NULL,
		EnrollAt DATETIME2 NOT NULL,
		Status INT NOT NULL CONSTRAINT DF_Enrollments_Status DEFAULT (0),
		CompletedAt DATETIME2 NULL,
		CancelledAt DATETIME2 NULL,
		ProgressPercentage DECIMAL(5, 2) NOT NULL CONSTRAINT DF_Enrollments_ProgressPercentage DEFAULT (0),

		CONSTRAINT FK_Enrollments_Users_UserId FOREIGN KEY (UserId)
			REFERENCES dbo.Users (Id) ON DELETE CASCADE,
		CONSTRAINT FK_Enrollments_Courses_CourseId FOREIGN KEY (CourseId)
			REFERENCES dbo.Courses (Id) ON DELETE CASCADE,
		CONSTRAINT CK_Enrollments_ProgressPercentage CHECK (ProgressPercentage >= 0 AND ProgressPercentage <= 100),
		CONSTRAINT CK_Enrollments_Status CHECK (Status IN (0, 1, 2))
	);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Enrollments_UserId_CourseId' AND object_id = OBJECT_ID(N'dbo.Enrollments'))
BEGIN
	CREATE UNIQUE INDEX IX_Enrollments_UserId_CourseId ON dbo.Enrollments (UserId, CourseId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Enrollments_UserId' AND object_id = OBJECT_ID(N'dbo.Enrollments'))
BEGIN
	CREATE INDEX IX_Enrollments_UserId ON dbo.Enrollments (UserId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Enrollments_CourseId' AND object_id = OBJECT_ID(N'dbo.Enrollments'))
BEGIN
	CREATE INDEX IX_Enrollments_CourseId ON dbo.Enrollments (CourseId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Enrollments_EnrollAt' AND object_id = OBJECT_ID(N'dbo.Enrollments'))
BEGIN
	CREATE INDEX IX_Enrollments_EnrollAt ON dbo.Enrollments (EnrollAt);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Enrollments_Status' AND object_id = OBJECT_ID(N'dbo.Enrollments'))
BEGIN
	CREATE INDEX IX_Enrollments_Status ON dbo.Enrollments (Status);
END;
GO

/* ==================== SAMPLE DATA (OPTIONAL) ==================== */
/*
	This block inserts deterministic sample records for manual testing.
	Safe to run multiple times (idempotent by known IDs or unique keys).

	Role enum:
	- 0: Student
	- 1: Instructor
	- 2: Admin

	Enrollment status enum:
	- 0: Active
	- 1: Completed
	- 2: Cancelled
*/

DECLARE @AdminUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @InstructorUserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @StudentUserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';

DECLARE @CourseDotnetApiId UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1';
DECLARE @CourseEfCoreId UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2';

DECLARE @EnrollmentStudentDotnetApiId UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1';
DECLARE @EnrollmentStudentEfCoreId UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2';

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = @AdminUserId)
BEGIN
	INSERT INTO dbo.Users (Id, UserName, Email, PasswordHash, Role, IsActive, CreatedAt)
	VALUES
	(
		@AdminUserId,
		N'admin.lms',
		N'admin@lms.local',
		N'$2a$11$7M4wo7f9kL2qjXQZ6gM6wOlQfK4v0sWJtC1MByxQ8iJfWkqj2B9mS',
		2,
		1,
		SYSUTCDATETIME()
	);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = @InstructorUserId)
BEGIN
	INSERT INTO dbo.Users (Id, UserName, Email, PasswordHash, Role, IsActive, CreatedAt)
	VALUES
	(
		@InstructorUserId,
		N'instructor.lms',
		N'instructor@lms.local',
		N'$2a$11$7M4wo7f9kL2qjXQZ6gM6wOlQfK4v0sWJtC1MByxQ8iJfWkqj2B9mS',
		1,
		1,
		SYSUTCDATETIME()
	);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = @StudentUserId)
BEGIN
	INSERT INTO dbo.Users (Id, UserName, Email, PasswordHash, Role, IsActive, CreatedAt)
	VALUES
	(
		@StudentUserId,
		N'student.lms',
		N'student@lms.local',
		N'$2a$11$7M4wo7f9kL2qjXQZ6gM6wOlQfK4v0sWJtC1MByxQ8iJfWkqj2B9mS',
		0,
		1,
		SYSUTCDATETIME()
	);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Courses WHERE Id = @CourseDotnetApiId)
BEGIN
	INSERT INTO dbo.Courses (Id, Title, Description, Price, Currency, CreatedBy, IsPublished, IsDeleted, CreatedAt)
	VALUES
	(
		@CourseDotnetApiId,
		N'Build REST APIs with ASP.NET Core',
		N'Hands-on course to design and build production-ready REST APIs.',
		49.99,
		N'USD',
		@InstructorUserId,
		1,
		0,
		SYSUTCDATETIME()
	);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Courses WHERE Id = @CourseEfCoreId)
BEGIN
	INSERT INTO dbo.Courses (Id, Title, Description, Price, Currency, CreatedBy, IsPublished, IsDeleted, CreatedAt)
	VALUES
	(
		@CourseEfCoreId,
		N'Entity Framework Core in Practice',
		N'Learn mappings, relationships, migrations, and performance basics.',
		39.50,
		N'USD',
		@InstructorUserId,
		1,
		0,
		SYSUTCDATETIME()
	);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Lessons WHERE Id = 'cccccccc-cccc-cccc-cccc-ccccccccccc1')
BEGIN
	INSERT INTO dbo.Lessons (Id, CourseId, Title, Content, [Order], IsDeleted)
	VALUES
	('cccccccc-cccc-cccc-cccc-ccccccccccc1', @CourseDotnetApiId, N'Introduction to Web APIs', N'Overview of API architecture and HTTP fundamentals.', 1, 0),
	('cccccccc-cccc-cccc-cccc-ccccccccccc2', @CourseDotnetApiId, N'Routing and Controllers', N'Build endpoints with controllers, routes, and action results.', 2, 0),
	('cccccccc-cccc-cccc-cccc-ccccccccccc3', @CourseEfCoreId, N'DbContext and Configuration', N'Configure entities and relationships using Fluent API.', 1, 0),
	('cccccccc-cccc-cccc-cccc-ccccccccccc4', @CourseEfCoreId, N'Queries and Performance', N'Use projection, filtering, and indexing for better performance.', 2, 0);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Enrollments WHERE Id = @EnrollmentStudentDotnetApiId)
BEGIN
	INSERT INTO dbo.Enrollments (Id, UserId, CourseId, EnrollAt, Status, CompletedAt, CancelledAt, ProgressPercentage)
	VALUES
	(
		@EnrollmentStudentDotnetApiId,
		@StudentUserId,
		@CourseDotnetApiId,
		DATEADD(DAY, -7, SYSUTCDATETIME()),
		0,
		NULL,
		NULL,
		45.00
	);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Enrollments WHERE Id = @EnrollmentStudentEfCoreId)
BEGIN
	INSERT INTO dbo.Enrollments (Id, UserId, CourseId, EnrollAt, Status, CompletedAt, CancelledAt, ProgressPercentage)
	VALUES
	(
		@EnrollmentStudentEfCoreId,
		@StudentUserId,
		@CourseEfCoreId,
		DATEADD(DAY, -20, SYSUTCDATETIME()),
		1,
		DATEADD(DAY, -2, SYSUTCDATETIME()),
		NULL,
		100.00
	);
END;

PRINT N'LMS database schema and sample data are ready.';

/* ==================== MANUAL TEST QUERIES ==================== */
/*
	Run these queries manually to validate data and constraints.
*/

-- 1) Row counts by table
SELECT N'Users' AS [TableName], COUNT(*) AS TotalRows FROM dbo.Users
UNION ALL
SELECT N'Courses', COUNT(*) FROM dbo.Courses
UNION ALL
SELECT N'Lessons', COUNT(*) FROM dbo.Lessons
UNION ALL
SELECT N'Enrollments', COUNT(*) FROM dbo.Enrollments;

-- 2) Courses with instructor and number of lessons
SELECT
	c.Id,
	c.Title,
	u.UserName AS Instructor,
	c.Price,
	c.Currency,
	COUNT(l.Id) AS LessonCount
FROM dbo.Courses c
INNER JOIN dbo.Users u ON u.Id = c.CreatedBy
LEFT JOIN dbo.Lessons l ON l.CourseId = c.Id AND l.IsDeleted = 0
WHERE c.IsDeleted = 0
GROUP BY c.Id, c.Title, u.UserName, c.Price, c.Currency
ORDER BY c.CreatedAt DESC;

-- 3) Student enrollment dashboard
SELECT
	u.UserName,
	u.Email,
	c.Title AS CourseTitle,
	e.Status,
	e.ProgressPercentage,
	e.EnrollAt,
	e.CompletedAt,
	e.CancelledAt
FROM dbo.Enrollments e
INNER JOIN dbo.Users u ON u.Id = e.UserId
INNER JOIN dbo.Courses c ON c.Id = e.CourseId
ORDER BY e.EnrollAt DESC;

-- 4) Validate unique enrollment (should return 0 rows)
SELECT
	e.UserId,
	e.CourseId,
	COUNT(*) AS DuplicateCount
FROM dbo.Enrollments e
GROUP BY e.UserId, e.CourseId
HAVING COUNT(*) > 1;

-- 5) Data quality checks (should return 0 rows)
SELECT * FROM dbo.Courses WHERE Price < 0;
SELECT * FROM dbo.Enrollments WHERE ProgressPercentage < 0 OR ProgressPercentage > 100;

-- 6) Optional negative test: constraints (run inside transaction and rollback)
/*
BEGIN TRAN;

-- Invalid role (expect CK_Users_Role failure)
INSERT INTO dbo.Users (Id, UserName, Email, PasswordHash, Role, IsActive, CreatedAt)
VALUES (NEWID(), N'invalid.role', N'invalid.role@lms.local', N'x', 9, 1, SYSUTCDATETIME());

-- Invalid progress (expect CK_Enrollments_ProgressPercentage failure)
INSERT INTO dbo.Enrollments (Id, UserId, CourseId, EnrollAt, Status, CompletedAt, CancelledAt, ProgressPercentage)
VALUES (NEWID(), @StudentUserId, @CourseDotnetApiId, SYSUTCDATETIME(), 0, NULL, NULL, 999.99);

ROLLBACK TRAN;
*/