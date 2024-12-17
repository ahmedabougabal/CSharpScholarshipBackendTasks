using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace eCommerce.Models
{
    [BsonIgnoreExtraElements]
    public class AppUser : MongoIdentityUser<string>
    {
        public AppUser()
        {
            Cart = new Cart();
            Orders = new List<Order>();
            CreatedAt = DateTime.UtcNow;
            LastLoginAt = DateTime.UtcNow;
        }

        [BsonElement("cart")]
        public Cart Cart { get; set; }

        [BsonElement("orders")]
        public ICollection<Order> Orders { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("lastLoginAt")]
        public DateTime LastLoginAt { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}