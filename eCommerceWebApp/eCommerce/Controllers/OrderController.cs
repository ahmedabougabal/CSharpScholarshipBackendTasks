using System;
using System.Threading.Tasks;
using eCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using MongoDB.Driver;
using eCommerce.Data;

namespace eCommerce.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MongoContext _context;

        public OrderController(
            UserManager<AppUser> userManager,
            MongoContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseProduct(string productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var product = await _context.Products.Find(p => p.Id.ToString() == productId).FirstOrDefaultAsync();
            if (product == null)
                return NotFound(new { message = "Product not found" });

            var order = new Order
            {
                UserId = user.Id,
                Products = new List<Product> { product },
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = product.Price
            };

            await _context.Orders.InsertOneAsync(order);

            return Ok(new { message = "Order created successfully", orderId = order.Id });
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var orders = await _context.Orders
                .Find(o => o.UserId == user.Id)
                .ToListAsync();

            // Load product details for each order
            foreach (var order in orders)
            {
                order.Products = await _context.Products
                    .Find(p => order.Products.Select(x => x.Id).Contains(p.Id))
                    .ToListAsync();
            }

            return Ok(orders);
        }
    }
}
