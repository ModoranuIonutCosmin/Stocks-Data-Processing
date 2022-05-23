namespace StocksProcessing.API.Payloads;


public class ApiResponse
{

    public bool Successful => ErrorMessage == null;

    public string ErrorMessage { get; set; }

    public object Response { get; set; }

}

public class ApiResponse<T> : ApiResponse
{
    public new T Response
    {
        get => (T) base.Response;
        set => base.Response = value;
    }

    public static ApiResponse<T> Of<T>(T contents)
    {
        return new()
        {
            Response = contents
        };
    }
}