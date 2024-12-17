using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace eCommerce.Models
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntention(decimal amount, string currency, string customerEmail, string customerFirstName, string customerLastName);
        string GetCheckoutUrl(string clientSecret);
    }

    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymentSettings _paymentSettings;

        public PaymentService(IHttpClientFactory httpClientFactory, IOptions<PaymentSettings> paymentSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _paymentSettings = paymentSettings.Value;
            _httpClient.BaseAddress = new Uri("https://accept.paymob.com/api/");
        }

        public async Task<string> CreatePaymentIntention(decimal amount, string currency, string customerEmail, string customerFirstName, string customerLastName)
        {
            try
            {
                // Step 1: Authentication Request
                var authRequest = new
                {
                    api_key = _paymentSettings.ApiKey
                };

                var authRequestJson = JsonSerializer.Serialize(authRequest);
                Console.WriteLine($"Auth Request URL: {_httpClient.BaseAddress}auth/tokens");
                Console.WriteLine($"Auth Request Body: {authRequestJson}");

                var authContent = new StringContent(
                    authRequestJson,
                    Encoding.UTF8,
                    "application/json"
                );

                Console.WriteLine("Step 1: Authentication Request");
                var authResponse = await _httpClient.PostAsync("auth/tokens", authContent);
                var authResponseContent = await authResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Auth Response Status: {authResponse.StatusCode}");
                Console.WriteLine($"Auth Response Headers: {string.Join(", ", authResponse.Headers)}");
                Console.WriteLine($"Auth Response Content: {authResponseContent}");

                if (!authResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Authentication failed. Status: {authResponse.StatusCode}, Response: {authResponseContent}");
                }

                var authResult = JsonSerializer.Deserialize<AuthResponse>(authResponseContent);
                
                if (authResult == null)
                {
                    Console.WriteLine("Auth Result is null after deserialization");
                    throw new Exception("Failed to deserialize authentication response");
                }

                var authToken = authResult.Token;
                Console.WriteLine($"Auth Token: {authToken}");

                if (string.IsNullOrEmpty(authToken))
                {
                    throw new Exception("Authentication token is empty");
                }

                // Step 2: Order Registration
                var orderRequest = new
                {
                    auth_token = authToken,
                    delivery_needed = "false",
                    amount_cents = (int)(amount * 100),
                    currency = currency,
                    items = new[]
                    {
                        new
                        {
                            name = "Order Payment",
                            amount_cents = (int)(amount * 100),
                            description = "Payment for order",
                            quantity = 1
                        }
                    }
                };

                var orderRequestJson = JsonSerializer.Serialize(orderRequest);
                Console.WriteLine($"Order Request URL: {_httpClient.BaseAddress}ecommerce/orders");
                Console.WriteLine($"Order Request Body: {orderRequestJson}");

                var orderContent = new StringContent(
                    orderRequestJson,
                    Encoding.UTF8,
                    "application/json"
                );

                Console.WriteLine("Step 2: Order Registration");
                var orderResponse = await _httpClient.PostAsync("ecommerce/orders", orderContent);
                var orderResponseContent = await orderResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Order Response Status: {orderResponse.StatusCode}");
                Console.WriteLine($"Order Response Headers: {string.Join(", ", orderResponse.Headers)}");
                Console.WriteLine($"Order Response Content: {orderResponseContent}");

                if (!orderResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Order registration failed. Status: {orderResponse.StatusCode}, Response: {orderResponseContent}");
                }

                var orderResult = JsonSerializer.Deserialize<OrderResponse>(orderResponseContent);

                // Step 3: Payment Key Generation
                var paymentKeyRequest = new
                {
                    auth_token = authToken,
                    amount_cents = (int)(amount * 100),
                    expiration = 3600,
                    order_id = orderResult?.Id,
                    billing_data = new
                    {
                        apartment = "NA",
                        email = customerEmail,
                        first_name = customerFirstName,
                        last_name = customerLastName,
                        phone_number = "+201234567890", 
                        street = "NA",
                        building = "NA",
                        floor = "NA",
                        state = "NA",
                        city = "Cairo", 
                        country = "EGY",
                        postal_code = "11511" 
                    },
                    currency = currency,
                    integration_id = _paymentSettings.IntegrationId,
                    lock_order_when_paid = "false",
                    profile_id = authResult.ProfileId
                };

                var paymentKeyRequestJson = JsonSerializer.Serialize(paymentKeyRequest);
                Console.WriteLine($"Payment Key Request URL: {_httpClient.BaseAddress}acceptance/payment_keys");
                Console.WriteLine($"Payment Key Request Body: {paymentKeyRequestJson}");

                var paymentKeyContent = new StringContent(
                    paymentKeyRequestJson,
                    Encoding.UTF8,
                    "application/json"
                );

                Console.WriteLine("Step 3: Payment Key Generation");
                var paymentKeyResponse = await _httpClient.PostAsync("acceptance/payment_keys", paymentKeyContent);
                var paymentKeyResponseContent = await paymentKeyResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Payment Key Response Status: {paymentKeyResponse.StatusCode}");
                Console.WriteLine($"Payment Key Response Headers: {string.Join(", ", paymentKeyResponse.Headers)}");
                Console.WriteLine($"Payment Key Response Content: {paymentKeyResponseContent}");

                if (!paymentKeyResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Payment key generation failed. Status: {paymentKeyResponse.StatusCode}, Response: {paymentKeyResponseContent}");
                }

                var paymentKeyResult = JsonSerializer.Deserialize<PaymentKeyResponse>(paymentKeyResponseContent);
                return paymentKeyResult?.Token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreatePaymentIntention: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public string GetCheckoutUrl(string token)
        {
            return $"https://accept.paymob.com/api/acceptance/iframes/886103?payment_token={token}";
        }
    }

    internal class AuthResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("profile_id")]
        public int ProfileId { get; set; }
    }

    internal class OrderResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    internal class PaymentKeyResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
