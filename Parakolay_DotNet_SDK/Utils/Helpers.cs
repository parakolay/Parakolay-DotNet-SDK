using System;
using System.Security.Cryptography;
using System.Text;

public static class Helpers
{
    public static long GetMilliseconds()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    public static string Generate(string message, string key)
    {
        var keyBytes = Convert.FromBase64String(key);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var hmacsha256 = new HMACSHA256(keyBytes);
        var hashmessage = hmacsha256.ComputeHash(messageBytes);
        return Convert.ToBase64String(hashmessage);
    }

    public static string GenerateSignature(string apiKey, string apiSecret, long nonce, string conversationId)
    {
        var message = $"{apiKey}{nonce}";
        var securityData = Generate(message, apiSecret);

        var secondMessage = $"{apiSecret}{conversationId}{nonce}{securityData}";
        var signature = Generate(secondMessage, apiSecret);

        return signature;
    }


    public static string GetClientIpAddress()
    {
        return "127.0.0.1";
    }
}