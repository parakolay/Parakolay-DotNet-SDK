using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Parakolay_DotNet_SDK
{
    public class Parakolay
    {
        private string version = "v1.0.2";

        private HttpClient multipartClient;
        private HttpClient jsonClient;

        private string apiKey;
        private string merchantNumber;
        private string conversationId;
        private string clientIpAddress;

        private long nonce;
        private string signature;

        private double amount;
        private string currency = "TRY";
        private string cardholderName;
        private string cardToken;
        private string threeDSessionID;

        public Parakolay(string baseUrl, string apiKey, string apiSecret, string merchantNumber, string conversationId, string clientIpAddress)
        {
            this.apiKey = apiKey;
            this.merchantNumber = merchantNumber;
            this.conversationId = conversationId;
            this.clientIpAddress = clientIpAddress;

            this.nonce = Helpers.GetMilliseconds();

            this.signature = Helpers.GenerateSignature(apiKey, apiSecret, this.nonce, conversationId);

            this.multipartClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                DefaultRequestHeaders =
            {
                { "User-Agent", "Parakolay .Net SDK " + this.version }
            }
            };

            this.jsonClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                DefaultRequestHeaders =
                {
                    { "User-Agent", "Parakolay .Net SDK " + this.version },
                    { "publicKey", apiKey },
                    { "nonce", this.nonce.ToString() },
                    { "signature", this.signature },
                    { "conversationId", conversationId },
                    { "clientIpAddress", clientIpAddress },
                    { "merchantNumber", merchantNumber }
                }
            };
        }

        public async Task<Init3dsResponseModel> Init3DS(string cardNumber, string cardholderName, string expireMonth, string expireYear, string cvc, double amount, int pointAmout, string callbackURL, string currency = "TRY", string languageCode = "TR")
        {
            this.cardToken = await GetCardToken(cardNumber, cardholderName, expireMonth, expireYear, cvc);
            this.threeDSessionID = await Get3DSession(amount, pointAmout, currency, languageCode);
            Init3dsResponseModel threedDinitResult = await Get3DInit(callbackURL, languageCode);

            threedDinitResult.cardToken = this.cardToken;
            threedDinitResult.threeDSessionID = this.threeDSessionID;

            threedDinitResult.amount = amount;
            threedDinitResult.cardHolderName = cardholderName;
            threedDinitResult.currency = this.currency;

            //TODO: Store these values in your state management solution or storage

            return threedDinitResult;
        }

        public async Task<PointsResult> GetPoints(string cardNumber, string cardholderName, string expireMonth, string expireYear, string cvc)
        {
            this.cardToken = await GetCardToken(cardNumber, cardholderName, expireMonth, expireYear, cvc);
            PointsResult result = await PointInquiry(this.cardToken);

            return result;
        }

        public async Task<ProvisionResult> Complete3DS(string threeDSessionID, double amount, string cardHolderName, string cardToken, string currency = "TRY")
        {
            string result = await Get3DSessionResult(threeDSessionID);
            if (result == "VerificationFinished")
                return await Provision(amount, cardHolderName, cardToken, threeDSessionID, currency);
            else
                return new ProvisionResult { errorMessage = "3D Secure process is not completed yet.", isSucceed = false };
        }

        private async Task<string> GetCardToken(string cardNumber, string cardholderName, string expireMonth, string expireYear, string cvc)
        {
            cardNumber = Regex.Replace(cardNumber, @"\s+", "");

            var data = new MultipartFormDataContent
            {
                { new StringContent(cardNumber), "CardNumber" },
                { new StringContent(expireMonth), "ExpireMonth" },
                { new StringContent(expireYear), "ExpireYear" },
                { new StringContent(cvc), "Cvv" },
                { new StringContent(this.apiKey), "PublicKey" },
                { new StringContent(this.nonce.ToString()), "Nonce" },
                { new StringContent(this.signature), "Signature" },
                { new StringContent(this.conversationId), "ConversationId" },
                { new StringContent(this.merchantNumber), "MerchantNumber" },
                { new StringContent(cardholderName), "CardHolderName" }
            };

            try
            {
                var response = await this.multipartClient.PostAsync("/v1/Tokens", data);
                var decodedResponse = JsonConvert.DeserializeObject<CardTokenResponseModel>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse!))
                    return decodedResponse!.cardToken;
                else
                    return JsonConvert.SerializeObject(new { error = decodedResponse!.errorMessage });
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new { error = e.Message });
            }
        }

        private async Task<string> Get3DSession(double amount, int pointAmount, string currency = "TRY", string languageCode = "TR")
        {
            this.amount = amount;
            this.currency = currency;

            var data = new
            {
                amount,
                pointAmount,
                this.cardToken,
                currency,
                paymentType = "Auth",
                installmentCount = 1,
                languageCode
            };

            try
            {
                var response = await this.jsonClient.PostAsync("/v1/threeds/getthreedsession", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                var decodedResponse = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse))
                    return decodedResponse!.threeDSessionId;
                else
                    return JsonConvert.SerializeObject(new { error = decodedResponse!.errorMessage });
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new { error = e.Message });
            }
        }

        private async Task<Init3dsResponseModel> Get3DInit(string callbackURL, string languageCode = "TR")
        {
            var data = new MultipartFormDataContent
            {
                { new StringContent(this.threeDSessionID), "ThreeDSessionId" },
                { new StringContent(callbackURL), "CallbackUrl" },
                { new StringContent(languageCode), "LanguageCode" },
                { new StringContent(this.clientIpAddress), "ClientIpAddress" },
                { new StringContent(this.apiKey), "PublicKey" },
                { new StringContent(this.nonce.ToString()), "Nonce" },
                { new StringContent(this.signature), "Signature" },
                { new StringContent(this.conversationId), "ConversationId" },
                { new StringContent(this.merchantNumber), "MerchantNumber" }
            };

            try
            {
                var response = await this.multipartClient.PostAsync("/v1/threeds/init3ds", data);
                var resultStr = await response.Content.ReadAsStringAsync();
                var decodedResponse = JsonConvert.DeserializeObject<Init3dsResponseModel>(resultStr);

                if (CheckError(decodedResponse!))
                    return decodedResponse!;
                else
                    return new Init3dsResponseModel { errorMessage = decodedResponse!.errorMessage };
            }
            catch (Exception e)
            {
                return new Init3dsResponseModel { errorMessage = e.Message };
            }
        }

        private async Task<string> Get3DSessionResult(string threeDSessionId, string languageCode = "TR")
        {
            var data = new
            {
                threeDSessionId,
                languageCode
            };

            try
            {
                var response = await this.jsonClient.PostAsync("/v1/threeds/getthreedsessionresult", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                var decodedResponse = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse))
                    return decodedResponse!.currentStep;
                else
                    return JsonConvert.SerializeObject(new { error = decodedResponse!.errorMessage });
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new { error = e.Message });
            }
        }

        private async Task<ProvisionResult> Provision(double amount, string cardHolderName, string cardToken, string threeDSessionId, string currency = "TRY")
        {
            var data = new
            {
                amount,
                cardToken,
                currency,
                paymentType = "Auth",
                cardHolderName,
                threeDSessionId
            };

            try
            {
                var response = await this.jsonClient.PostAsync("/v1/Payments/provision", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                var decodedResponse = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse))
                {
                    //TODO: Clear the session data set before.

                    return JsonConvert.SerializeObject(decodedResponse);
                }
                else
                {
                    return new ProvisionResult { errorMessage = decodedResponse!.errorMessage, isSucceed = false };
                }
            }
            catch (Exception e)
            {
                return new ProvisionResult { errorMessage = e.Message, isSucceed = false };
            }
        }

        public async Task<ReverseResult> Reverse(string orderid, string languageCode = "TR")
        {
            var data = new
            {
                orderid,
                languageCode,
            };

            try
            {
                var response = await this.jsonClient.PostAsync("/v1/Payments/reverse", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                var decodedResponse = JsonConvert.DeserializeObject<ReverseResult>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse))
                {
                    return decodedResponse;
                }
                else
                {
                    return new ReverseResult { errorMessage = decodedResponse!.errorMessage, isSucceed = false };
                }
            }
            catch (Exception e)
            {
                return new ReverseResult { errorMessage = e.Message, isSucceed = false };
            }
        }

        public async Task<ReturnResult> Return(double amount, string orderid, string languageCode = "TR")
        {
            var data = new
            {
                amount,
                orderid,
                languageCode,
            };

            try
            {
                var response = await this.jsonClient.PostAsync("/v1/Payments/return", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                var decodedResponse = JsonConvert.DeserializeObject<ReturnResult>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse))
                {
                    return decodedResponse;
                }
                else
                {
                    return new ReturnResult { errorMessage = decodedResponse!.errorMessage, isSucceed = false };
                }
            }
            catch (Exception e)
            {
                return new ReturnResult { errorMessage = e.Message, isSucceed = false };
            }
        }

        public async Task<BINInfoResult> BINInfo(string binNumber, string languageCode = "TR")
        {
            var data = new
            {
                binNumber,
                languageCode,
            };

            try
            {
                var response = await this.jsonClient.PostAsync("/v1/Payments/bin-information", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                var decodedResponse = JsonConvert.DeserializeObject<BINInfoResult>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse))
                {
                    return decodedResponse;
                }
                else
                {
                    return new BINInfoResult { errorMessage = decodedResponse!.errorMessage, isSucceed = false };
                }
            }
            catch (Exception e)
            {
                return new BINInfoResult { errorMessage = e.Message, isSucceed = false };
            }
        }

        public async Task<dynamic> Installment(string binNumber, string merchantNumber, double amount)
        {
            var data = new
            {
                binNumber,
                merchantNumber,
                amount,
            };

            try
            {
                var response = await this.jsonClient.GetAsync("/v1/Installment?binNumber=" + binNumber + "&amount=" + amount + "&merchantNumber=" + merchantNumber);
                var decodedResponse = JsonConvert.DeserializeObject<InstallmentResult>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse))
                {
                    return decodedResponse;
                }
                else
                {
                    return new InstallmentResult { errorMessage = decodedResponse!.errorMessage, isSucceed = false };
                }
            }
            catch (Exception e)
            {
                return new InstallmentResult { errorMessage = e.Message, isSucceed = false };
            }
        }

        private async Task<PointsResult> PointInquiry(string cardToken, string languageCode = "TR", string currency = "TRY")
        {
            var data = new
            {
                cardToken,
                languageCode,
                currency,
            };

            try
            {
                var response = await this.jsonClient.PostAsync("/v1/Payments/pointInquiry", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                var decodedResponse = JsonConvert.DeserializeObject<PointsResult>(await response.Content.ReadAsStringAsync());

                if (CheckError(decodedResponse))
                {
                    return decodedResponse;
                }
                else
                {
                    return new PointsResult { errorMessage = decodedResponse!.errorMessage, isSucceed = false };
                }
            }
            catch (Exception e)
            {
                return new PointsResult { errorMessage = e.Message, isSucceed = false };
            }
        }

        private static bool CheckError(dynamic data)
        {
            if (data.isSucceed == true)
                return true;
            else
                return false;
        }
    }
}