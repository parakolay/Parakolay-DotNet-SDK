public class InstallmentResult
{
    public string merchantNumber { get; set; }
    public List<LoyaltyInstallmentPricing> loyaltyInstallmentPricing { get; set; }
    public bool isSucceed { get; set; }
    public string conversationId { get; set; }
    public string errorMessage { get; set; }
}

public class InstallmentPricing
{
    public string profileCardType { get; set; }
    public int installmentNumber { get; set; }
    public int installmentNumberEnd { get; set; }
    public double commissionRate { get; set; }
    public double amount { get; set; }
    public double commissionAmount { get; set; }
    public double totalAmount { get; set; }
    public int blockedDayNumber { get; set; }
    public bool isActive { get; set; }
}

public class LoyaltyInstallmentPricing
{
    public string cardNetwork { get; set; }
    public List<InstallmentPricing> installmentPricings { get; set; }
}