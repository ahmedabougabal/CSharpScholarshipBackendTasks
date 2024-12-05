namespace eCommerce.Models
{
    public class PaymentSettings
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
        public bool IsTestMode { get; set; }
        public int IntegrationId { get; set; }
    }
}
