using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace StocksProccesing.Relational.Encryption.Personal_Data;

public class LookupProtectorKeyRing : ILookupProtectorKeyRing {
    private readonly IConfiguration _configuration;

    public string this[string keyId] => _configuration["PersonalDataKeys:Key"];

    public string CurrentKeyId => "master_key_id";

    public LookupProtectorKeyRing(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<string> GetAllKeyIds() {
        return new string[] { "master_key_id" };
    }
}