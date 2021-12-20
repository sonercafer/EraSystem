using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; 
using System.Threading;
using System.Threading.Tasks;

namespace Era.Data.Base
{
    public interface IGenericRepository<TEntity>
    {
        ValueTask<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        IQueryable<TEntity> GetListAsync(CancellationToken cancellationToken = default);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateNonEventAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<bool> ExecuteNonQuery(string sql, CancellationToken cancellationToken = default);
    }
}
