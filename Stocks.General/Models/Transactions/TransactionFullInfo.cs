using System;

namespace Stocks.General.Models.Transactions;

public class TransactionFullInfo
{
    public int Id { get; set; }

    public string Ticker { get; set; }

    public bool IsBuy { get; set; }

    public decimal UnitsPurchased { get; set; }

    public decimal InitialPrice { get; set; }

    public decimal CurrentPrice { get; set; }

    public decimal InvestedAmount { get; set; }

    public decimal StopLossAmount { get; set; }

    public decimal TakeProfitAmount { get; set; }

    public double Leverage { get; set; }

    public DateTimeOffset Date { get; set; }

    public decimal Value => InitialPrice * UnitsPurchased + ProfitOrLoss;

    public bool IsCFD { get; set; }

    public decimal ProfitOrLoss { get; set; }

    public decimal ProfitOrLossPercentage { get; set; }
}