using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassasystemet.Models
{
    public class Receipt
    {       
        public List<ReceiptRow> receiptRowsList = new List<ReceiptRow>();
        
        public decimal GetTotal()
        {
            decimal sum = 0;
            foreach (var row in receiptRowsList)
            {
                sum += row.GetTotal();
            }
            return sum;
        }
        public void AddToReceipt(ReceiptRow receiptRow)
        {
            //foreach (var row in receiptRowsList)
            //{
            //    if(row.ProductId == productId)
            //    {
            //        row.AddCount(count);
            //    }
                
                receiptRowsList.Add(receiptRow);
            //}
        }
        
    }

}
