using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Interfaces
{
    public interface IRepository<TEntity, TKey>
        where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TKey id);
    }
}
