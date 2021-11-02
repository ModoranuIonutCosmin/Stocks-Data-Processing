namespace StocksProccesing.Relational.Repositories
{
    public interface IEFRepository<T>
    {
        public T _dbContext { get; set; }
    }
}