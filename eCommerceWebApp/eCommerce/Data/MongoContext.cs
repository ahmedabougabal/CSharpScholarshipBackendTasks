using MongoDB.Driver;
using eCommerce.Models;
using Microsoft.Extensions.Configuration;
using AspNetCore.Identity.MongoDbCore.Infrastructure;

namespace eCommerce.Data
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoConnection"]);
            _database = client.GetDatabase("eCommerce");
        }

        public IMongoDatabase Database => _database;
    }
}