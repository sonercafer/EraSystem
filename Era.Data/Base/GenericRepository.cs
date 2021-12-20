using Era.Core.Event;
using Era.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Era.Data.Base
{
    class GenericRepository
    {
    }
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly EraContext _db;

        private readonly IEnumerable<IConsumer<EntityAdded<TEntity>>> _consumersAdded;
        private readonly IEnumerable<IConsumer<EntityModified<TEntity>>> _consumersModified;
        private readonly IEnumerable<IConsumer<EntityDeleted<TEntity>>> _consumersDeleted;

        public GenericRepository(EraContext db, IEnumerable<IConsumer<EntityAdded<TEntity>>> consumersAdded, IEnumerable<IConsumer<EntityModified<TEntity>>> consumersModified, IEnumerable<IConsumer<EntityDeleted<TEntity>>> consumersDeleted)
        {
            _db = db;
            _consumersAdded = consumersAdded;
            _consumersModified = consumersModified;
            _consumersDeleted = consumersDeleted;
        }

        public async ValueTask<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.FindAsync<TEntity>(id, cancellationToken);
        }

        public IQueryable<TEntity> GetListAsync(CancellationToken cancellationToken = default)
        {
            return _db.Set<TEntity>();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Set<TEntity>().CountAsync(predicate, cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _db.Set<TEntity>().AddAsync(entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            var tasks = _consumersAdded.Select(q => q.Handle(new EntityAdded<TEntity>(entity), cancellationToken));
            await Task.WhenAll(tasks);
            return entity;
        }

        public async Task<TEntity> UpdateNonEventAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _db.Set<TEntity>().Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _db.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            foreach (var entity in entities)
            {
                var tasks = _consumersAdded.Select(q => q.Handle(new EntityAdded<TEntity>(entity), cancellationToken = default));
                await Task.WhenAll(tasks);
            }

            return true;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _db.Set<TEntity>().Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync(cancellationToken);

            var tasks = _consumersModified.Select(q => q.Handle(new EntityModified<TEntity>(entity), cancellationToken));
            await Task.WhenAll(tasks);

            return entity;
        }

        public async Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _db.Set<TEntity>().Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);
            var tasks = _consumersDeleted.Select(q => q.Handle(new EntityDeleted<TEntity>(entity), cancellationToken));
            await Task.WhenAll(tasks);
            return true;
        }

        public async Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _db.Set<TEntity>().RemoveRange(entities);
            await _db.SaveChangesAsync(cancellationToken);
            foreach (var entity in entities)
            {
                var tasks = _consumersDeleted.Select(q => q.Handle(new EntityDeleted<TEntity>(entity), cancellationToken));
                await Task.WhenAll(tasks);
            }

            return true;
        }

        public async Task<bool> ExecuteNonQuery(string sql, CancellationToken cancellationToken = default)
        {
            await _db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
            return true;
        }
    }
}
