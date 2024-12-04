using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Text.Json;

namespace eCommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MongoContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(MongoContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("ProductsController initialized");
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                _logger.LogInformation("Retrieving all products");
                var products = await _context.Products.Find(_ => true).ToListAsync();
                _logger.LogInformation("Retrieved {Count} products", products.Count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            try
            {
                _logger.LogInformation("Retrieving product with ID: {Id}", id);
                var product = await _context.Products.Find(p => p.Id.ToString() == id).FirstOrDefaultAsync();

                if (product == null)
                {
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {Id}", id);
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductDetails}", JsonSerializer.Serialize(product));
                await _context.Products.InsertOneAsync(product);
                _logger.LogInformation("Product created successfully with ID: {Id}", product.Id);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {Id}", id);
                var result = await _context.Products.ReplaceOneAsync(p => p.Id.ToString() == id, product);

                if (result.ModifiedCount == 0)
                {
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                _logger.LogInformation("Product updated successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {Id}", id);
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {Id}", id);
                var result = await _context.Products.DeleteOneAsync(p => p.Id.ToString() == id);

                if (result.DeletedCount == 0)
                {
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                _logger.LogInformation("Product deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {Id}", id);
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }
    }
}
