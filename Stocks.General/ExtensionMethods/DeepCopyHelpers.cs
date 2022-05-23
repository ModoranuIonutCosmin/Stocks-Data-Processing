using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Stocks.General.ExtensionMethods;

public static class DeepCopyHelpers
{
    public static T DeepClone<T>(this T objectToClone)
    {
        using MemoryStream stream = new MemoryStream();
        
        BinaryFormatter formatter = new BinaryFormatter();
        
        formatter.Serialize(stream, objectToClone);
        stream.Position = 0;
        
        return (T) formatter.Deserialize(stream);
    }
}