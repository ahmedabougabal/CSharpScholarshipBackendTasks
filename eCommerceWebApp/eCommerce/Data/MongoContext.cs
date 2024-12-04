using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using eCommerce.Models;

namespace eCommerce.Data
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoContext> _logger;
        private readonly IMongoCollection<Product> _products;

        public MongoContext(IConfiguration configuration, ILogger<MongoContext> logger)
        {
            _logger = logger;
            try
            {
                var connectionString = configuration.GetConnectionString("MongoConnection");
                _logger.LogInformation("Attempting to connect to MongoDB");

                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = "mongodb://localhost:27017";
                    _logger.LogWarning("MongoDB connection string not found in configuration, using default: {ConnectionString}", connectionString);
                }
                
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase("eCommerce");
                _products = _database.GetCollection<Product>("Products");
                
                // Ensure indexes
                CreateIndexes();
                
                _logger.LogInformation("Successfully connected to MongoDB");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MongoDB connection");
                throw;
            }
        }

        private void CreateIndexes()
        {
            try
            {
                var indexKeysDefinition = Builders<Product>.IndexKeys.Ascending(p => p.Name);
                var createIndexOptions = new CreateIndexOptions { Unique = false };
                var createIndexModel = new CreateIndexModel<Product>(indexKeysDefinition, createIndexOptions);
                _products.Indexes.CreateOne(createIndexModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating indexes");
            }
        }

        public IMongoCollection<Product> Products => _products;
    }
}