using Stocks.General.Entities;

namespace Stocks.General.Models.MyProfile;

public class ProfilePrivateData
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public decimal Capital { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public Subscription Subscription { get; set; }
}