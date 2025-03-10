using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Training.Common.Helpers;
using Training.DataAccess.DbContexts;
using Training.DataAccess.IEntities;
using Training.Repository.Repositories;

namespace Training.Repository.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChanges();

        IBaseRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;

        MyDbContext DbContext { get; }
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(MyDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._repositories = new();
            this.DbContext = this._context;
            this._httpContextAccessor = httpContextAccessor;
        }

        public MyDbContext DbContext { get; private set; }

        public IBaseRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            var type = typeof(BaseRepository<TEntity>);

            if (!_repositories.TryGetValue(type, out var value))
            {
                value = new BaseRepository<TEntity>(_context);
                _repositories[type] = value;
            }

            return (IBaseRepository<TEntity>)value;
        }

        public async Task<int> SaveChanges()
        {
            SaveChangesInternal();

            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._context?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void SaveChangesInternal()
        {
            var entries = _context.ChangeTracker.Entries()
                .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToArray();
            if (entries.Length == 0) return;

            SaveChangesInternal(entries, EntityState.Added);
            SaveChangesInternal(entries, EntityState.Modified);

            var deletedEntries = _context.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Deleted);
            SaveChangesSoftDelete(deletedEntries);
        }

        private void SaveChangesInternal(EntityEntry[] entries, EntityState state)
        {
            // Enforce type defaults for all entities
            foreach (var item in entries)
            {
                foreach (var p in item.Properties)
                {
                    if (p.CurrentValue == null)
                    {
                        continue;
                    }

                    switch (p.Metadata.ClrType.Name)
                    {
                        case "String": // Replace all empty strings with null
                            var emptyString = string.IsNullOrWhiteSpace(p.CurrentValue.ToString());
                            p.CurrentValue = emptyString ? null : p.CurrentValue;
                            break;
                    }
                }
            }

            foreach (var item in entries.Where(t => t.State == state))
            {
                PropertyEntry? propertyEntry;
                if (state == EntityState.Added)
                {
                    // CreatedBy
                    propertyEntry = item.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedBy");
                    if (propertyEntry != null)
                    {
                        propertyEntry.CurrentValue = _httpContextAccessor.HttpContext?.User.Claims.GetUserIdNullable() ?? 0;
                    }

                    // CreatedAt
                    propertyEntry = item.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                    if (propertyEntry != null)
                    {
                        propertyEntry.CurrentValue = DateTimeHelper.GetDtOffset();
                    }
                }


                // UpdatedBy
                propertyEntry = item.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedBy");
                if (propertyEntry != null)
                {
                    propertyEntry.CurrentValue = _httpContextAccessor.HttpContext?.User.Claims.GetUserIdNullable() ?? 0;
                }

                // UpdatedAt
                propertyEntry = item.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
                if (propertyEntry != null)
                {
                    propertyEntry.CurrentValue = DateTimeHelper.GetDtOffset();
                }

                // Trim String Entries Before Saving
                var propertyValues = item.Properties
                    .Where(p => p.CurrentValue is string && !string.IsNullOrEmpty(Convert.ToString(p.CurrentValue)));
                foreach (var propertyValue in propertyValues)
                {
                    propertyValue.CurrentValue = (propertyValue.CurrentValue?.ToString() ?? string.Empty).Trim();
                }
            }
        }

        private void SaveChangesSoftDelete(IEnumerable<EntityEntry> entries)
        {
            foreach (var item in entries)
            {
                if (item.Entity is not IIsDeletedEntity entity) continue;

                // Set the entity to unchanged (if we mark the whole entity as Modified, every field gets sent to Db as an update)
                item.State = EntityState.Unchanged;

                // Only update the IsDeleted flag - only this will get sent to the Db
                entity.IsDeleted = true;

                if (item.Entity is not IBaseEntity baseEntity) continue;

                baseEntity.UpdatedBy = _httpContextAccessor.HttpContext?.User.Claims.GetUserIdNullable() ?? 0;
                baseEntity.UpdatedAt = DateTimeHelper.GetDtOffset();
            }
        }
    }
}
