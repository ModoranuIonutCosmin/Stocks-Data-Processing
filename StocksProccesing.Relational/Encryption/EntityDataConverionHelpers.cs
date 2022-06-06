using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StocksProccesing.Relational.Encryption;

public static class ProtectedDataConverterEF
{
    public static void AddDataProtectionConverters<T>(this DbContext context,
        EntityTypeBuilder<T> entityBuilder)
        where T: class, new()
    {
        IEnumerable<PropertyInfo> stringProps = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        foreach (PropertyInfo property in stringProps)
        {
            EncryptDataConverter encryptConverter = new EncryptDataConverter(
                context.GetService<IDataProtectionProvider>());

            entityBuilder.Property<string>(property.Name)
                .HasConversion(encryptConverter);
        }
    }

}