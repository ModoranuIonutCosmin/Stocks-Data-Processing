using Stocks.General.Models.TradeSuggestions;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services
{
    public interface ITradeSuggestionsService
    {
        Task<List<TradeSuggestion>> DetermineViableTrades(ApplicationUser userRequesting, string ticker, string algorithm, TimeSpan interval);
    }
}