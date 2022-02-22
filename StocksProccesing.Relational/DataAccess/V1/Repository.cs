using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StocksProccesing.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        protected readonly StocksMarketContext _dbContext;
        protected IDbContextTransaction dbContextTransaction;

        public Repository(StocksMarketContext context)
        {
            _dbContext = context;
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            await _dbContext.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllWhereAsync
            (Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>()
                .Where(predicate).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _dbContext.FindAsync<TEntity>(id);
        }

        public async Task DeleteWhereAsync(Func<TEntity, bool> predicate)
        {
            if(predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            _dbContext.RemoveRange(_dbContext.Set<TEntity>().Where(predicate));

            await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public void StartNewTransaction()
        {
            dbContextTransaction = _dbContext.Database.BeginTransaction();
        }

        public async Task EndLastTransactionAsync()
        {
            await _dbContext.SaveChangesAsync();

            if (dbContextTransaction != null)
                dbContextTransaction.Commit();

            dbContextTransaction = null;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
