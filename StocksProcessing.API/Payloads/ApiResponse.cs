namespace StocksProcessing.API.Payloads;


public class ApiResponse
{

    public bool Successful => ErrorMessage == null;

    public string ErrorMessage { get; set; }

    public object Response { get; set; }

}