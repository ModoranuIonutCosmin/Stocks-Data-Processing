using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksProcessing.API.Models
{
    public class DailyShortSummaryCriteria
    {
        public int SimpleDate { get; set; }
        public string Ticker { get; set; }
    }
}
