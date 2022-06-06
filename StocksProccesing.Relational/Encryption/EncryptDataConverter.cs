using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StocksProccesing.Relational.Encryption;

public class EncryptDataConverter : ValueConverter<string, string>
{
    public EncryptDataConverter(IDataProtectionProvider dataProtectionProvider)
        : base(cellValue => dataProtectionProvider
                .CreateProtector("gdpr_compliance")
                .Protect(cellValue),
            cellValue => dataProtectionProvider
                .CreateProtector("gdpr_compliance")
                .Unprotect(cellValue),
            default) 
    { }
}