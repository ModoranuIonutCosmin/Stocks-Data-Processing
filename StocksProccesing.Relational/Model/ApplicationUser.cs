using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model
{
    public class ApplicationUser : IdentityUser
    {
        #region Public Properties

        /// <summary>
        /// The users first name
        /// </summary>
        [Column(TypeName = "VARCHAR(50)")]

        public string FirstName { get; set; }

        /// <summary>
        /// The users last name
        /// </summary>
        [Column(TypeName = "VARCHAR(50)")]
        public string LastName { get; set; }

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
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(UserName);
            hash.Add(Email);
            hash.Add(ConcurrencyStamp);
            return hash.ToHashCode();
        }

        #endregion
    }
}
