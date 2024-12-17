using System.Text;
using System.Text.Json;
using eCommerce.Models;
using eCommerce.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace eCommerce.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymentSettings _paymentSettings;

        public PaymentService(IHttpClientFactory httpClientFactory, IOptions<PaymentSettings> paymentSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _paymentSettings = paymentSettings.Value;
            _httpClient.BaseAddress = new Uri("https://accept.paymob.com/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_paymentSettings.SecretKey}");
        }

        public async Task<string> CreatePaymentIntention(decimal amount, string currency, string customerEmail, string customerFirstName, string customerLastName)
        {
            var paymentRequest = new
            {
                amount = (int)(amount * 100), // Convert to smallest currency unit
                currency = currency,
                payment_methods = new[] { "card" },
                items = new[]
                {
                    new
                    {
                        name = "Order Payment",
                        amount = (int)(amount * 100),
                        description = "Payment for order",
                        quantity = 1
                    }
                },
                billing_data = new
                {
                    apartment = "NA",
                    email = customerEmail,
                    first_name = customerFirstName,
                    last_name = customerLastName,
                    phone_number = "NA",
                    street = "NA",
                    building = "NA",
                    floor = "NA",
                    state = "NA",
                    country = "NA"
                },
                customer = new
                {
                    first_name = customerFirstName,
                    last_name = customerLastName,
                    email = customerEmail
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(paymentRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("v1/intention/", content);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create payment intention. Status code: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<PaymentIntentionResponse>(responseContent);
            
            return responseData?.ClientSecret;
        }

        public string GetCheckoutUrl(string clientSecret)
        {
            return $"https://accept.paymob.com/unifiedcheckout/?publicKey={_paymentSettings.PublicKey}&clientSecret={clientSecret}";
        }
    }

    internal class PaymentIntentionResponse
    {
        public string ClientSecret { get; set; }
    }
}
