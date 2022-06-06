using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace StocksProccesing.Relational.Model;

public class ApplicationUser : IdentityUser
{
    [Column(TypeName = "VARCHAR(256)")]
    [ProtectedPersonalData]
    public string FirstName { get; set; }

    [Column(TypeName = "VARCHAR(256)")]
    [ProtectedPersonalData]
    public string LastName { get; set; }
    
    [Column(TypeName = "decimal(20, 4)")] 
    public decimal Capital { get; set; }


    public List<StocksTransaction> OpenTransactions { get; set; }

    public override bool Equals(object obj)
    {
        return obj is ApplicationUser user &&
               Id == user.Id &&
               UserName == user.UserName &&
               Email == user.Email &&
               ConcurrencyStamp == user.ConcurrencyStamp;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(UserName);
        hash.Add(Email);
        hash.Add(ConcurrencyStamp);
        return hash.ToHashCode();
    }

}