namespace StocksFinalSolution.BusinessLogic.Security;

public interface ISimpleAESProtector
{
    string Encrypt(string plainText);
    string Decrypt (string encryptedText);
}