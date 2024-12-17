using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using eCommerce.Data;

namespace eCommerce.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MongoContext _context;

        public PaymentController(
            UserManager<AppUser> userManager,
            MongoContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("process/{orderId}")]
        public async Task<IActionResult> ProcessPayment(string orderId, [FromBody] PaymentRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var order = await _context.Orders.Find(o => o.Id.ToString() == orderId).FirstOrDefaultAsync();
            if (order == null)
                return NotFound(new { message = "Order not found" });

            if (order.Status != OrderStatus.Pending)
                return BadRequest(new { message = "Order is not in pending status" });

            order.Status = OrderStatus.Processing;
            await _context.Orders.ReplaceOneAsync(o => o.Id.ToString() == orderId, order);

            // Here you would integrate with a real payment gateway
            // For demonstration, we'll just mark the order as delivered
            var update = Builders<Order>.Update
                .Set("Status", OrderStatus.Delivered)
                .Set("PaymentMethod", request.PaymentMethod);
            
            await _context.Orders.UpdateOneAsync(o => o.Id.ToString() == orderId, update);

            return Ok(new { message = "Payment processed successfully" });
        }
    }

    public class PaymentRequest
    {
        [Required]
        public required string PaymentMethod { get; set; }
        // Add other payment-related properties as needed
        // Such as card details, billing address, etc.
    }
}
