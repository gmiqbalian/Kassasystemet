using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassasystemet.Models
{
    public class Product
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public Product(string productId, string name, decimal price, string priceType)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            PriceType = priceType;
        }
    }
}
