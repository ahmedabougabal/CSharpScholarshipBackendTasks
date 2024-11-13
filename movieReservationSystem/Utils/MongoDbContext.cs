using MongoDB.Driver;
using movieReservationSystem.Models;

namespace movieReservationSystem.Utils
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Movie> Movies => _database.GetCollection<Movie>("Movies");
        public IMongoCollection<Reservation> Reservations => _database.GetCollection<Reservation>("Reservations");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}