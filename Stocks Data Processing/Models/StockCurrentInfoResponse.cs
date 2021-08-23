using System;

namespace Stocks_Data_Processing.Models
{
    /// <summary>
    /// Grupare ce tine datele obtinute in legatura
    /// cu valorile curente ale stock-urilor
    /// obtinute prin diverse metode si
    /// daca obtinerea s-a efectuat cu success.
    /// </summary>
    public class StockCurrentInfoResponse
    {
        /// <summary>
        /// Daca sursa ce a extras datele a reusit sau nu
        /// </summary>
        public bool Successful { get => Exception == null; }

        /// <summary>
        /// Exceptia ce indica motivul esuarii ( sau null daca e caz de success )
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Simbolul unei companii pe piata de stock-uri
        /// </summary>
        public StocksTicker Ticker { get; set; }

        /// <summary>
        /// Pretul de pornire a valorii stock-ului pe acea zi
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// Valoarea curenta a unui stock
        /// </summary>
        public double Current { get; set; }

        /// <summary>
        /// Data si ora la care a fost observata valoarea preturilor
        /// </summary>
        public DateTimeOffset DateTime { get; set; }
    }
}
