public class ReturnResult
{
    public object returnMessage { get; set; }
    public string approvalStatus { get; set; }
    public object provisionNumber { get; set; }
    public bool isSucceed { get; set; }
    public string errorCode { get; set; }
    public string errorMessage { get; set; }
    public string conversationId { get; set; }
}