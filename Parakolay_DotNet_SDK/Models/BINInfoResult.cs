public class BINInfoResult
{
    public string binNumber { get; set; }
    public string cardBrand { get; set; }
    public string cardNetwork { get; set; }
    public string cardType { get; set; }
    public string cardSubType { get; set; }
    public int bankCode { get; set; }
    public string bankName { get; set; }
    public bool isVirtual { get; set; }
    public bool isSucceed { get; set; }
    public string errorCode { get; set; }
    public string errorMessage { get; set; }
    public string conversationId { get; set; }
}