namespace StocksFinalSolution.BusinessLogic.Interfaces.Repositories.Base
{
    public interface IBaseEntity<TKey>
    {
        TKey Id { get; set; }
    }
}