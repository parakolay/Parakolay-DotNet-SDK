public class ReverseResult
{ 
    public string orderId { get; set; }
    public string provisionNumber { get; set; }
    public bool isSucceed { get; set; }
    public object errorCode { get; set; }
    public object errorMessage { get; set; }
    public string conversationId { get; set; }
}