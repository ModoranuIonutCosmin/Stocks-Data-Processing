using Microsoft.AspNetCore.Identity;
namespace StocksProccesing.Relational.Encryption.Personal_Data;

public class PersonalDataProtector : IPersonalDataProtector
{
    private readonly ILookupProtector _lookupProtector;
    private readonly ILookupProtectorKeyRing _lookupProtectorKeyRing;

    public PersonalDataProtector(ILookupProtector lookupProtector, ILookupProtectorKeyRing lookupProtectorKeyRing)
    {
        _lookupProtector = lookupProtector;
        _lookupProtectorKeyRing = lookupProtectorKeyRing;
    }
    public string Protect(string data)
    {
        var currentKeyId = _lookupProtectorKeyRing.CurrentKeyId;

        return _lookupProtector.Protect(currentKeyId, data);
    }

    public string Unprotect(string data)
    {
        var currentKeyId = _lookupProtectorKeyRing.CurrentKeyId;

        return _lookupProtector.Unprotect(currentKeyId, data);
    }
}