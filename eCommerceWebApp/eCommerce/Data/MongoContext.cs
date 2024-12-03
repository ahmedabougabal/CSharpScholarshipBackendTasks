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

        public MongoContext(IConfiguration configuration, ILogger<MongoContext> logger)
        {
            _logger = logger;
            try
            {
                var connectionString = configuration["MongoConnection"];
                _logger.LogInformation("Attempting to connect to MongoDB");

                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("MongoDB connection string is not configured");
                    throw new InvalidOperationException("MongoDB connection string is not configured");
                }
                
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase("eCommerce");
                
                // Test the connection
                _database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}").Wait();
                _logger.LogInformation("Successfully connected to MongoDB");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MongoDB connection");
                throw;
            }
        }

        public IMongoDatabase Database => _database;
    }
}