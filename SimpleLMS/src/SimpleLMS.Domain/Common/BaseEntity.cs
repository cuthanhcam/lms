using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }
        public bool IsDeleted { get; protected set; }

        public virtual void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        protected void MarkAsModified()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
