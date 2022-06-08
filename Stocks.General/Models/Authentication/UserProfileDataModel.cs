﻿using Stocks.General.Entities;

namespace Stocks.General.Models.Authentication;

public class UserProfileDetailsApiModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Token { get; set; }

    public Subscription Subscription { get; set; }
}