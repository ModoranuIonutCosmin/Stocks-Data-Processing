using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.DataAccess.V1.Decorator
{
    public class StockSummariesRepositoryDecorator : Repository<StocksOhlc, int>, IStockSummariesRepository
    {
        private readonly IStockSummariesRepository stockSummariesRepository;

        public StockSummariesRepositoryDecorator(StocksMarketContext context, 
            IStockSummariesRepository stockSummariesRepository) : base(context)
        {
            this.stockSummariesRepository = stockSummariesRepository;
        }

        public virtual async Task<List<StocksOhlc>> GetAllEntriesByTickerAndPeriod(string ticker, TimeSpan period)
        {
            return await stockSummariesRepository.GetAllEntriesByTickerAndPeriod(ticker, period);
        }

        public virtual async Task<List<StocksOhlc>> GetLastSummaryEntryForAllTickers(TimeSpan interval)
        {
            return await stockSummariesRepository.GetLastSummaryEntryForAllTickers(interval);
        }

        public virtual StocksOhlc GetLastSummaryEntryForTicker(string ticker, TimeSpan interval)
        {
            return stockSummariesRepository.GetLastSummaryEntryForTicker(ticker, interval);
        }

        public virtual async Task<StocksOhlc> GetLastSummaryEntryForTickerAndInterval(string ticker, TimeSpan interval)
        {
            return await stockSummariesRepository.GetLastSummaryEntryForTickerAndInterval(ticker, interval);
        }
    }
}
