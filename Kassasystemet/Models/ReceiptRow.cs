using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassasystemet.Models
{
    public class ReceiptRow
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public ReceiptRow(string productId, string productName, int count, decimal price)
        {
            ProductId = productId;
            ProductName = productName;
            Count = count;
            Price = price;
        }
        public decimal GetRowTotal()
        {
            
            return Count * Price;
        }
        public void AddCount(int count)
        {
            Count += count;
        }
    }
}
