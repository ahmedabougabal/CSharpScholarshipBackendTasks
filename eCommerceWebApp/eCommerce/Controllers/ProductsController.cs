using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace eCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly MongoContext _context;

        public ProductsController(MongoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var collection = _context.Database.GetCollection<Product>("Products");
            var products = await collection.Find(_ => true).ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var collection = _context.Database.GetCollection<Product>("Products");
            var product = await collection.Find(p => p.Id == id).FirstOrDefaultAsync();
            
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {
                if (product.Id == 0)
                {
                    product.Id = new Random().Next(1, 100000);
                }

                var collection = _context.Database.GetCollection<Product>("Products");
                await collection.InsertOneAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                return BadRequest($"A product with ID {product.Id} already exists.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var collection = _context.Database.GetCollection<Product>("Products");
            var result = await collection.ReplaceOneAsync(p => p.Id == id, product);

            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var collection = _context.Database.GetCollection<Product>("Products");
            var result = await collection.DeleteOneAsync(p => p.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
