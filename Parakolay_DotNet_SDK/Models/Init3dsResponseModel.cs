public class Init3dsResponseModel
{
    public string conversationId { get; set; }
    public bool isSucceed { get; set; }
    public string errorCode { get; set; }
    public string errorMessage { get; set; }
    public string htmlContent { get; set; }
    public string integrationId { get; set; }

    public string cardToken { get; set; }
    public string threeDSessionID { get; set; }

    public double amount { get; set; }
    public string cardHolderName { get; set; }
    public string currency { get; set; }
}