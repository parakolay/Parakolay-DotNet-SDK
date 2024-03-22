using System;
using System.Collections.Generic;
using Parakolay_DotNet_SDK;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "YOUR_API_KEY";
        string apiSecret = "YOUR_API_SECRET";
        string merchantNumber = "YOUR_MERCHANT_NUMBER";
        string conversationId = "YOUR_ORDER_ID";
        string clientIpAddress = "CLIENT_IP_ADDRESS";

        string baseUrl = "https://api.parakolay.com";
        string testUrl = "https://api-test.parakolay.com";

        int amount = 1;
        int pointAmount = 0;

        Parakolay apiClient = new Parakolay(baseUrl, apiKey, apiSecret, merchantNumber, conversationId, clientIpAddress);

        if (args.Length == 0)
        {
            Init3dsResponseModel result = await apiClient.Init3DS("CARD_NUMBER", "CARDHOLDER_NAME", "EXPIRE_MONTH (MM)", "EXPIRE_YEAR (YY)", "CVV", amount, pointAmount, "YOUR_CALLBACK_URL");
            Console.WriteLine(result.cardToken);
            Console.WriteLine(result.threeDSessionID);
            Console.WriteLine(result.htmlContent);
        }
        else if (args[0] == "complete")
        {
            ProvisionResult complete = await apiClient.Complete3DS("3DS_SESSION_ID", amount, "CARDHOLDER_NAME", "CARD_TOKEN");
            Console.WriteLine(complete);
        }
        else if (args[0] == "reverse")
        {
            ReverseResult reverse = await apiClient.Reverse("ORDER_ID");
            Console.WriteLine(reverse);
        }
        else if (args[0] == "return")
        {
            ReturnResult ret = await apiClient.Return(amount, "ORDER_ID");
            Console.WriteLine(ret);
        }
        else if (args[0] == "bininfo")
        {
            BINInfoResult ret = await apiClient.BINInfo("BIN_NUMBER (First 6 or 8 digits)");
            Console.WriteLine(ret);
        }
        else if (args[0] == "installment")
        {
            InstallmentResult ret = await apiClient.Installment("BIN_NUMBER (First 6 or 8 digits)", merchantNumber, amount);
            Console.WriteLine(ret);
        }
        else if (args[0] == "pointInquiry")
        {
            var ret = await apiClient.GetPoints("CARD_NUMBER", "CARDHOLDER_NAME", "EXPIRE_MONTH (MM)", "EXPIRE_YEAR (YY)", "CVV");
            Console.WriteLine(ret);
        }
    }
}