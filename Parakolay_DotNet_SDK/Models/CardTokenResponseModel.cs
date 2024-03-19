public class CardTokenResponseModel
{
    public string cardToken { get; set; }
    public string signature { get; set; }
    public bool isSucceed { get; set; }
    public string errorCode { get; set; }
    public string errorMessage { get; set; }
}