using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public AppUser? User { get; set; }

        public List<Product> Products { get; set; } = new();

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public PaymentMethods PaymentMethod { get; set; }
    }
}