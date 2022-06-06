using System;
using System.Security.Cryptography;
using System.Text;

namespace StocksFinalSolution.BusinessLogic.Security;

public class SimpleAESProtector : ISimpleAESProtector
{
    private readonly string _key;
    private readonly string _iv;

    public SimpleAESProtector(string key, string iv)
    {
        _key = key;
        _iv = iv;
    }
    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();

        byte[] iv = Encoding.UTF8.GetBytes(_iv);
        byte[] key = ExpandKeyGivenToHash(_key);

        using var aesEncryptor = aes.CreateEncryptor(key, iv);

        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cypherTextBytes = aesEncryptor.TransformFinalBlock(plainTextBytes, 0,
            plainTextBytes.Length);

        // return Encoding.UTF8.GetString(cypherTextBytes);
        return Convert.ToBase64String(cypherTextBytes);
    }
    
    public string Decrypt (string encryptedText)
    {
        using var aes = Aes.Create();

        byte[] iv = Encoding.UTF8.GetBytes(_iv);
        byte[] key = ExpandKeyGivenToHash(_key);

        using var aesDecryptor = aes.CreateDecryptor(key, iv);

        byte[] cypherTextBytes = Convert.FromBase64String(encryptedText);
        byte[] plainTextBytes = aesDecryptor.TransformFinalBlock(cypherTextBytes, 0,
            cypherTextBytes.Length);

        // return Encoding.UTF8.GetString(plainTextBytes);
        return Encoding.UTF8.GetString(plainTextBytes);
    }
    
    
    
    private static byte[] ExpandKeyGivenToHash(string password)
    {
        var keyBytes = Encoding.UTF8.GetBytes(password);

        using var md5 = MD5.Create();
        
        return md5.ComputeHash(keyBytes);
    }
}