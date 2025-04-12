using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashSmart.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CashSmart.Core.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            UpdateTimeStamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimeStamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimeStamps()
        {
            var timeStampedEntities = new HashSet<Type>
            {
                typeof(User),
            };

            var entries = ChangeTracker.Entries()
                .Where(e => timeStampedEntities.Contains(e.Entity.GetType()) && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    SetPropertyValue(entry.Entity, "CreatedAt", DateTime.UtcNow);
                }

                SetPropertyValue(entry.Entity, "UpdatedAt", DateTime.UtcNow);
            }
        }

        private void SetPropertyValue(object entity, string propertyName, object value)
        {
            var property = entity.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, value);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.HasIndex(u => u.UserName)
                    .IsUnique();

                entity.Property(u => u.Id)
                    .HasDefaultValueSql("gen_random_uuid()");
            });
        }

    }
}