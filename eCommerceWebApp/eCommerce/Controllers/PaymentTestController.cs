using Microsoft.AspNetCore.Mvc;
using eCommerce.Models;
using Microsoft.AspNetCore.Authorization;

namespace eCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTestController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentTestController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("test-payment")]
        public async Task<IActionResult> TestPayment()
        {
            try
            {
                // Create a payment intention with test data
                var paymentToken = await _paymentService.CreatePaymentIntention(
                    amount: 123.45m,  // Specific amount for easy identification
                    currency: "EGP",
                    customerEmail: "test@example.com",
                    customerFirstName: "Test",
                    customerLastName: "User"
                );

                // Get the checkout URL
                var checkoutUrl = _paymentService.GetCheckoutUrl(paymentToken);

                return Ok(new { 
                    message = "Payment intention created successfully",
                    checkoutUrl = checkoutUrl,
                    paymentToken = paymentToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
