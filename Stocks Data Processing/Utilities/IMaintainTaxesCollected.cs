using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public interface IMaintainTaxesCollected
    {
        Task CollectTaxes();
    }
}