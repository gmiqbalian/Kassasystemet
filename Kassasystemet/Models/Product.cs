﻿using System;
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

        public decimal Price
        {
            get
            {
                if (ProductId == "100" && DateTime.Now > new DateTime(2022, 11, 20) && DateTime.Now < new DateTime(2022, 11, 31))
                    return price = 12;

                return price;
            }
            set { price = value; }

        }
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
