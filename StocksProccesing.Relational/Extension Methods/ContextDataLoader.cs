using Stocks.General;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Linq;

namespace StocksProccesing.Relational.Extension_Methods
{
    public static class ContextDataLoader
    {
        public static void EnsureCompaniesDataExists(this StocksMarketContext stocksContext)
        {

            ///Obtine lista acestor companii din enumul <see cref="StocksTicker"/>
            var companies = Enum.GetValues(typeof(StocksTicker)).Cast<StocksTicker>()
                                    .Select(s => s.ToString()).ToList();


            if (!stocksContext.Companies.Any())
            //Daca tabelul companiilor (din BD) nu contine nicio companie...
            {
                //... introduce toate companiile urmarite.
                stocksContext.Companies.AddRange(companies.Select(ticker => new Company()
                {
                    Name = $"{ticker} company",
                    Ticker = ticker
                }));
            }
        }
    }
}
