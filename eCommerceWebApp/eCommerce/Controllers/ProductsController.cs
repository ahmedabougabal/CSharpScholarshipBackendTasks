using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
                _logger.LogInformation("Received GET request to retrieve products");
                var collection = _context.Database.GetCollection<Product>("Products");
                var products = await collection.Find(_ => true).ToListAsync();
                _logger.LogInformation("Products retrieved successfully");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                _logger.LogInformation("Received GET request to retrieve product: {Id}", id);
                var collection = _context.Database.GetCollection<Product>("Products");
                var product = await collection.Find(p => p.Id == id).FirstOrDefaultAsync();
                
                if (product == null)
                {
                    _logger.LogInformation("Product not found: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Product retrieved successfully: {Id}", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Test endpoint
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Test endpoint is working!");
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            _logger.LogInformation("Received POST request to create product: {@Product}", product);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state");
                    return BadRequest(ModelState);
                }

                if (product.Id == 0)
                {
                    product.Id = new Random().Next(1, 100000);
                    _logger.LogInformation("Generated new product ID: {Id}", product.Id);
                }

                var collection = _context.Database.GetCollection<Product>("Products");
                _logger.LogInformation("Attempting to insert product into database");
                await collection.InsertOneAsync(product);
                
                _logger.LogInformation("Product created successfully: {Id}", product.Id);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                _logger.LogWarning("Duplicate product ID: {Id}", product.Id);
                return BadRequest($"A product with ID {product.Id} already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                _logger.LogInformation("Received PUT request to update product: {Id}", id);
                if (id != product.Id)
                {
                    _logger.LogWarning("Invalid product ID");
                    return BadRequest();
                }

                var collection = _context.Database.GetCollection<Product>("Products");
                var result = await collection.ReplaceOneAsync(p => p.Id == id, product);

                if (result.ModifiedCount == 0)
                {
                    _logger.LogInformation("Product not found: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Product updated successfully: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("Received DELETE request to delete product: {Id}", id);
                var collection = _context.Database.GetCollection<Product>("Products");
                var result = await collection.DeleteOneAsync(p => p.Id == id);

                if (result.DeletedCount == 0)
                {
                    _logger.LogInformation("Product not found: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Product deleted successfully: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
