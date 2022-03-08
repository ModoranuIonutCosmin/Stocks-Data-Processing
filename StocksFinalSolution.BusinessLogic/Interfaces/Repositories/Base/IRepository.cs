using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base
{
    public interface IRepository<TEntity, TKey>
        where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(List<TEntity> entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllWhereAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetByIdAsync(TKey id);
        Task DeleteWhereAsync(Func<TEntity, bool> predicate);
        void StartNewTransaction();
        Task EndLastTransactionAsync();
        Task SaveChangesAsync();
    }
}
