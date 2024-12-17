using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using eCommerce.Models;

namespace eCommerce.Data
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<AppUser> _users;
        private readonly ILogger<MongoContext> _logger;

        public IMongoCollection<Product> Products => _products;
        public IMongoCollection<Order> Orders => _orders;
        public IMongoCollection<AppUser> Users => _users;

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
                _orders = _database.GetCollection<Order>("Orders");
                _users = _database.GetCollection<AppUser>("Users");
                
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
                // Product indexes
                var productIndexKeysDefinition = Builders<Product>.IndexKeys.Ascending(p => p.Name);
                var productCreateIndexOptions = new CreateIndexOptions { Unique = false };
                var productCreateIndexModel = new CreateIndexModel<Product>(productIndexKeysDefinition, productCreateIndexOptions);
                _products.Indexes.CreateOne(productCreateIndexModel);

                // User indexes
                var userEmailIndexKeysDefinition = Builders<AppUser>.IndexKeys.Ascending(u => u.Email);
                var userEmailCreateIndexOptions = new CreateIndexOptions { Unique = true };
                var userEmailCreateIndexModel = new CreateIndexModel<AppUser>(userEmailIndexKeysDefinition, userEmailCreateIndexOptions);
                _users.Indexes.CreateOne(userEmailCreateIndexModel);

                var userUsernameIndexKeysDefinition = Builders<AppUser>.IndexKeys.Ascending(u => u.UserName);
                var userUsernameCreateIndexOptions = new CreateIndexOptions { Unique = true };
                var userUsernameCreateIndexModel = new CreateIndexModel<AppUser>(userUsernameIndexKeysDefinition, userUsernameCreateIndexOptions);
                _users.Indexes.CreateOne(userUsernameCreateIndexModel);

                // Order indexes
                var orderUserIdIndexKeysDefinition = Builders<Order>.IndexKeys.Ascending(o => o.UserId);
                var orderCreateIndexOptions = new CreateIndexOptions { Unique = false };
                var orderCreateIndexModel = new CreateIndexModel<Order>(orderUserIdIndexKeysDefinition, orderCreateIndexOptions);
                _orders.Indexes.CreateOne(orderCreateIndexModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating indexes");
            }
        }
    }
}