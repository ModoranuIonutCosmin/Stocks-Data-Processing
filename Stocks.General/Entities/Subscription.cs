
using System;
using System.ComponentModel.DataAnnotations;
using Stocks.General.DataModels;

namespace Stocks.General.Entities;
public class Subscription
{
    [Key]
    [MaxLength(300)]
    public string Id { get; set; }
    [MaxLength(300)]
    public string CustomerId { get; set; }

    public SubscriptionType Type { get; set; }

    public DateTimeOffset PeriodEnd { get; set; }
    public DateTimeOffset PeriodStart { get; set; }

    [MaxLength(50)]
    public string Status { get; set; }
}