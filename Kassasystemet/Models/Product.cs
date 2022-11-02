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
        private decimal price;
        public string PriceType { get; set; }
        public int Stock { get; set; }
        public Product(string productId, string name, decimal price, string priceType, int stock)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            PriceType = priceType;
            Stock = stock;
        }
        public decimal Price
        {
            get
            {
                if (ProductId == "100" && DateTime.Now > new DateTime(2022, 11, 10) && DateTime.Now < new DateTime(2022, 11, 12))
                    return price = 12;
                else if (ProductId == "100" && DateTime.Now > new DateTime(2022, 11, 13) && DateTime.Now < new DateTime(2022, 11, 15))
                    return price = 15;
                else
                    return price;
            }
            set { price = value; }

        }        
    }
}
