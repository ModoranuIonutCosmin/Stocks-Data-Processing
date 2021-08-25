using System;
using System.Collections.Generic;
using System.Text;

namespace StocksProccesing.Relational
{
    public static class DatabaseSettings
    {
        public static string ConnectionString
            = "Server=.;Database=stocksDb;Trusted_Connection=True;MultipleActiveResultSets=true";
    }
}
