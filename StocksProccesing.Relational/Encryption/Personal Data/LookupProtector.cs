using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StocksFinalSolution.BusinessLogic.Security;

namespace StocksProccesing.Relational.Encryption.Personal_Data;

public class LookupProtector : ILookupProtector
{
    private readonly ILookupProtectorKeyRing _keyRing;
    private readonly IConfiguration _configuration;

    public LookupProtector(ILookupProtectorKeyRing keyRing,
        IConfiguration configuration)
    {
        _keyRing = keyRing;
        _configuration = configuration;
    }
    
    public string Protect(string keyId, string data)
    {
        string key = _keyRing[keyId];
        string iv = Environment.GetEnvironmentVariable("PersonalDataKeys:IV") ??
            _configuration["PersonalDataKeys:IV"];

        return new SimpleAESProtector(key, iv).Encrypt(data);
    }

    public string Unprotect(string keyId, string data) {
        string key = _keyRing[keyId];
        string iv = Environment.GetEnvironmentVariable("PersonalDataKeys:IV") ??
            _configuration["PersonalDataKeys:IV"];

        return new SimpleAESProtector(key, iv).Decrypt(data);
    }
}