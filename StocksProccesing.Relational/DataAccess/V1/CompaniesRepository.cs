using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess.V1
{
    public class CompaniesRepository : Repository<Company, int>, ICompaniesRepository
    {
        public CompaniesRepository(StocksMarketContext context) : base(context)
        {
        }

        public Company GetCompanyData(string ticker)
        {
            return _dbContext.Companies.FirstOrDefault(c => c.Ticker == ticker);
        }

        public List<Company> GetAllStocksCompanies()
        {
            return _dbContext.Companies.ToList();
        }

        public async Task<Company> GetPredictionsByTicker(string ticker)
        {
            return await _dbContext.Companies
                .Where(e => e.Ticker == ticker)
                .Include(e => e.PricesData
                              .Where(e => e.Prediction)
                              .OrderBy(e => e.Date))
                .FirstOrDefaultAsync();
        }

        public void EnsureCompaniesDataExists()
        {
            if (!_dbContext.Companies.Any())
            //Daca tabelul companiilor (din BD) nu contine nicio companie...
            {
                //... introduce toate companiile urmarite.
                _dbContext.Companies.AddRange(new List<Company>()
                {
                    new Company()
                    {
                        Ticker = "TSLA",
                        Name = "Tesla Motors, Inc",
                        UrlLogo = "https://upload.wikimedia.org/wikipedia/commons/e/e8/Tesla_logo.png",
                        Description = @"Tesla was founded in 2003 by a group of engineers who wanted to prove that people didn’t need to compromise to drive electric – that electric vehicles can be better, quicker and more fun to drive than gasoline cars. Today, Tesla builds not only all-electric vehicles but also infinitely scalable clean energy generation and storage products. Tesla believes the faster the world stops relying on fossil fuels and moves towards a zero-emission future, the better.

Launched in 2008, the Roadster unveiled Tesla’s cutting-edge battery technology and electric powertrain. From there, Tesla designed the world’s first ever premium all-electric sedan from the ground up – Model S – which has become the best car in its class in every category. Combining safety, performance, and efficiency, Model S has reset the world’s expectations for the car of the 21st century with the longest range of any electric vehicle, over-the-air software updates that make it better over time, and a record 0-60 mph acceleration time of 2.28 seconds as measured by Motor Trend. In 2015, Tesla expanded its product line with Model X, the safest, quickest and most capable sport utility vehicle in history that holds 5-star safety ratings across every category from the National Highway Traffic Safety Administration. Completing CEO Elon Musk’s “Secret Master Plan,” in 2016, Tesla introduced Model 3, a low-priced, high-volume electric vehicle that began production in 2017. Soon after, Tesla unveiled the safest, most comfortable truck ever – Tesla Semi – which is designed to save owners at least $200,000 over a million miles based on fuel costs alone. In 2019, Tesla unveiled Model Y, a mid-size SUV, with seating for up to seven, and Cybertruck, which will have better utility than a traditional truck and more performance than a sports car.

Tesla vehicles are produced at its factory in Fremont, California, and Gigafactory Shanghai. To achieve our goal of having the safest factories in the world, Tesla is taking a proactive approach to safety, requiring production employees to participate in a multi-day training program before ever setting foot on the factory floor. From there, Tesla continues to provide on-the-job training and track performance daily so that improvements can be made quickly. The resu"
                    },
                    new Company()
                    {
                        Ticker = "INTC",
                        Name = "Intel",
                        UrlLogo = "https://www.intel.com/content/dam/logos/logo-energyblue-1x1.png",
                        Description = "Intel Corp. engages in the design, manufacture, and sale of computer products and technologies. It delivers computer, networking, data storage, and communications platforms. The firm operates through the following segments: Client Computing Group (CCG), Data Center Group (DCG), Internet of Things Group (IOTG), Non-Volatile Memory Solutions Group (NSG), Programmable Solutions (PSG), and All Other. The CCG segment consists of platforms designed for notebooks, 2-in-1 systems, desktops, tablets, phones, wireless and wired connectivity products, and mobile communication components. The DCG segment includes workload-optimized platforms and related products designed for enterprise, cloud, and communication infrastructure market. The IOTG segment offers compute solutions for targeted verticals and embedded applications for the retail, manufacturing, health care, energy, automotive, and government market segments. The NSG segment is composed of NAND flash memory products primarily used in solid-state drives. The PSG segment contains programmable semiconductors and related products for a broad range of markets, including communications, data center, industrial, military, and automotive."
                    },
                    new Company()
                    {
                        Ticker = "MS",
                        Name = "Morgan Stanley",
                        UrlLogo = "https://dynl.mktgcdn.com/p/JU19rlnTKZs3wRuCOg4p2E8LCnNOFzEpDEvo3KwE6Vg/1207x1208.jpg",
                        Description = "Morgan Stanley operates as a global financial services company. The firm provides investment banking products and services to its clients and customers including corporations, governments, financial institutions, and individuals. It operates through the following segments: Institutional Securities, Wealth Management, and Investment Management. The Institutional Services segment provides financial advisory, capital-raising services, and related financing services on behalf of institutional investors. The Wealth Management segment offers brokerage and investment advisory services covering various types of investments, including equities, options, futures, foreign currencies, precious metals, fixed-income securities, mutual funds, structured products, alternative investments, unit investment trusts, managed futures, separately managed accounts, and mutual fund asset allocation programs. The Investment Management segment provides equity, fixed income, alternative investments, real estate, and merchant banking strategies. The company was founded by Harold Stanley and Henry S. Morgan in 1924 and is headquartered in New York, NY."
                    },
                    new Company()
                    {
                        Ticker = "MSFT",
                        Name = "Microsoft",
                        UrlLogo = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/Microsoft_logo.svg/1024px-Microsoft_logo.svg.png",
                        Description = "Microsoft Corporation, leading developer of personal-computer software systems and applications. The company also publishes books and multimedia titles, produces its own line of hybrid tablet computers, offers e-mail services, and sells electronic game systems and computer peripherals (input/output devices). It has sales offices throughout the world. In addition to its main research and development centre at its corporate headquarters in Redmond, Washington, U.S., Microsoft operates research labs in Cambridge, England (1997); Beijing, China (1998); Bengaluru, India (2005); Cambridge, Massachusetts (2008); New York, New York (2012); and Montreal, Canada (2015)."
                    },
                    new Company()
                    {
                        Ticker = "VOO",
                        Name = "Vanguard S&P 500 ETF",
                        UrlLogo = "https://etoro-cdn.etorostatic.com/market-avatars/4238/150x150.png",
                        Description = "The investment seeks to track the performance of the Standard & Poor‘s 500 Index that measures the investment return of large-capitalization stocks. The fund employs an indexing investment approach designed to track the performance of the Standard & Poor's 500 Index, a widely recognized benchmark of U.S. stock market performance that is dominated by the stocks of large U.S. companies. The advisor attempts to replicate the target index by investing all, or substantially all, of its assets in the stocks that make up the index, holding each stock in approximately the same proportion as its weighting in the index."
                    },
                });
                _dbContext.SaveChanges();
            }
        }
    }
}
