using Stocks.General;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
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
                stocksContext.Companies.AddRange(new List<Company>()
                {
                    new Company()
                    {
                        Ticker = "TSLA",
                        Name = "Tesla Motors, Inc",
                        UrlLogo = "https://upload.wikimedia.org/wikipedia/commons/e/e8/Tesla_logo.png",
                        Description = ""
                    },
                    new Company()
                    {
                        Ticker = "INTC",
                        Name = "Intel",
                        UrlLogo = "https://www.intel.com/content/dam/logos/logo-energyblue-1x1.png",
                        Description = ""
                    },
                    new Company()
                    {
                        Ticker = "MS",
                        Name = "Morgan Stanley",
                        UrlLogo = "https://dynl.mktgcdn.com/p/JU19rlnTKZs3wRuCOg4p2E8LCnNOFzEpDEvo3KwE6Vg/1207x1208.jpg",
                        Description = ""
                    },
                    new Company()
                    {
                        Ticker = "PFE",
                        Name = "Pfizer",
                        UrlLogo = "https://logowik.com/content/uploads/images/880_pfizer.jpg",
                        Description = ""
                    },
                    new Company()
                    {
                        Ticker = "MSFT",
                        Name = "Microsoft",
                        UrlLogo = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/Microsoft_logo.svg/1024px-Microsoft_logo.svg.png",
                        Description = ""
                    },
                    new Company()
                    {
                        Ticker = "T",
                        Name = "AT&T Inc",
                        UrlLogo = "https://logodownload.org/wp-content/uploads/2018/04/att-logo-0.png",
                        Description = ""
                    },
                    new Company()
                    {
                        Ticker = "VOO",
                        Name = "Vanguard S&P 500 ETF",
                        UrlLogo = "https://etoro-cdn.etorostatic.com/market-avatars/4238/150x150.png",
                        Description = ""
                    },
                });

            }
        }
    }
}
