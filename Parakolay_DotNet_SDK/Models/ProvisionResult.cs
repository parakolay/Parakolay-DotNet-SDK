public class ProvisionResult
{
    public string orderId { get; set; }
    public string provisionNumber { get; set; }
    public bool isSucceed { get; set; }
    public string errorCode { get; set; }
    public string errorMessage { get; set; }
    public string conversationId { get; set; }
}