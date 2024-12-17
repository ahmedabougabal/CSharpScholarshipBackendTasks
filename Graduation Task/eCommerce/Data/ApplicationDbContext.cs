using MongoDB.Driver;
using eCommerce.Models;
using Microsoft.Extensions.Configuration;

namespace eCommerce.Data
{
    public class ApplicationDbContext
    {
        private readonly IMongoDatabase _database;

        public ApplicationDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("eCommerce");
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
    }
}
