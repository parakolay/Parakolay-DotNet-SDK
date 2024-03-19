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
        Init3dsResponseModel result =new Init3dsResponseModel();

        if (args.Length == 0 || args[0] != "return")
        {
            result = await apiClient.Init3DS("CARD_NUMBER", "CARDHOLDER_NAME", "EXPIRE_MONTH (MM)", "EXPIRE_YEAR (YY)", "CVV", amount, pointAmount, "YOUR_CALLBACK_URL");
            Console.WriteLine(result);
        }
        else
        {
            var complete = apiClient.Complete3DS(result);
            Console.WriteLine(complete);
        }
    }
}