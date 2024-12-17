namespace eCommerce.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntention(decimal amount, string currency, string customerEmail, string customerFirstName, string customerLastName);
        string GetCheckoutUrl(string clientSecret);
    }
}
