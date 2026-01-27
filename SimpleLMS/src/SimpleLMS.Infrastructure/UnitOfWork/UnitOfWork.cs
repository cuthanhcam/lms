using Microsoft.EntityFrameworkCore.Storage;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Infrastructure.Persistence;
using SimpleLMS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        // Repositories
        private IUserRepository? _userRepository;
        private ICourseRepository? _courseRepository;
        private ILessonRepository? _lessonRepository;
        private IEnrollmentRepository? _enrollmentRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users
        {
            get
            {
                _userRepository ??= new UserRepository(_context);
                return _userRepository;
            }
        }

        public ICourseRepository Courses
        {
            get
            {
                _courseRepository ??= new CourseRepository(_context);
                return _courseRepository;
            }
        }

        public ILessonRepository Lessons
        {
            get
            {
                _lessonRepository ??= new LessonRepository(_context);
                return _lessonRepository;
            }
        }

        public IEnrollmentRepository Enrollments
        {
            get
            {
                _enrollmentRepository ??= new EnrollmentRepository(_context);
                return _enrollmentRepository;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
