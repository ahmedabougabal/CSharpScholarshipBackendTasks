using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [Required]
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string? Description { get; set; }

        [Required]
        [BsonElement("Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("InStock")]
        public bool InStock { get; set; } = true;
    }
}
