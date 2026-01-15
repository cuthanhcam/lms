USE LMS_Dev;

CREATE TABLE Users (
	Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	UserName NVARCHAR(100) NOT NULL,
	Email NVARCHAR(255) NOT NULL,
	PasswordHash NVARCHAR(500) NOT NULL,
	Role NVARCHAR(50) NOT NULL CHECK (Role IN('Admin', 'Instructor', 'Student')),
	IsActive BIT NOT NULL DEFAULT 1,
	CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE UNIQUE INDEX UX_Users_Email ON Users (Email);

CREATE TABLE Courses (
	Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	Title NVARCHAR(200) NOT NULL,
	Description NVARCHAR(MAX) NULL,
	Price DECIMAL(18,2) NOT NULL CHECK (Price > 0),
	CreatedBy UNIQUEIDENTIFIER NOT NULL,
	IsPublished BIT NOT NULL DEFAULT 0,
	IsDeleted BIT NOT NULL DEFAULT 0,
	CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

	CONSTRAINT FK_Courses_Users 
		FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);

CREATE TABLE Lessons (
	Id UNIQUEIDENTIFIER	NOT NULL PRIMARY KEY DEFAULT NEWID(),
	CourseId UNIQUEIDENTIFIER NOT NULL,
	Title NVARCHAR(200) NOT NULL,
	Content NVARCHAR(MAX) NOT NULL,
	[Order] INT NOT NULL,
	IsDeleted BIT NOT NULL DEFAULT 0,

	CONSTRAINT FK_Lessons_Courses
		FOREIGN KEY (CourseId) REFERENCES Courses(Id)
);

CREATE TABLE Enrollments (
	Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	UserId UNIQUEIDENTIFIER NOT NULL,
	CourseId UNIQUEIDENTIFIER NOT NULL,
	EnrollAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

	CONSTRAINT FK_Enrollments_Users
		FOREIGN KEY (UserId) REFERENCES Users(Id),

	CONSTRAINT FK_Enrollments_Courses
		FOREIGN KEY (CourseId) REFERENCES Courses(Id),

	CONSTRAINT UQ_Enrollments_User_Course
		UNIQUE (UserId, CourseId)
);

CREATE INDEX IX_Courses_CreatedBy ON Courses (CreatedBy);
CREATE INDEX IX_Courses_IsPublished ON Courses (IsPublished);
CREATE INDEX IX_Lessons_CourseId ON Lessons (CourseId);
CREATE INDEX IX_Enrollments_UserId ON Enrollments (UserId);

-- Admin
INSERT INTO Users (UserName, Email, PasswordHash, Role)
VALUES ('admin', 'admin@gmail.com', 'hash', 'Admin');

-- Instructor
INSERT INTO Users (UserName, Email, PasswordHash, Role)
VALUES ('teacher1', 'teacher@gmail.com', 'hash', 'Instructor');

-- Student
INSERT INTO Users (UserName, Email, PasswordHash, Role)
VALUES ('student1', 'student@gmail.com', 'hash', 'Student');

SELECT *
FROM Courses
WHERE IsPublished = 1 AND IsDeleted = 0