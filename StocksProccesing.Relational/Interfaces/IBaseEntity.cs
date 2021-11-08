namespace Application.Interfaces
{
    public interface IBaseEntity<TKey>
    {
        TKey Id { get; set; }
    }
}