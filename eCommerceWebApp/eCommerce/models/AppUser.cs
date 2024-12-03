using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using AspNetCore.Identity.MongoDbCore.Models;

namespace eCommerce.Models
{
    public class AppUser : MongoIdentityUser<string>
    {
        public AppUser()
        {
            Cart = new Cart();
            Orders = new List<Order>();
        }

        public int? CartId { get; set; }
        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}