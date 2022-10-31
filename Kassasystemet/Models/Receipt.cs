using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Kassasystemet.Models
{
    public class Receipt
    {   
        public string ReceiptNumber { get;}
        public List<ReceiptRow> receiptRowsList = new List<ReceiptRow>();
        private static int receiptNumberSeed = 100;
        public Receipt()
        {
            ReceiptNumber = "RN-" + receiptNumberSeed.ToString();
            receiptNumberSeed++;
        }
        public decimal GetReceiptTotal()
        {
            decimal sum = 0;
            foreach (var row in receiptRowsList)
            {
                sum += row.GetRowTotal();
            }
            return sum;
        }
        public void AddToReceipt(ReceiptRow receiptRow) //ask for help
        {
            foreach (var row in receiptRowsList)
            {
                if (row.ProductId == receiptRow.ProductId)
                {
                    row.AddCount(receiptRow.Count);
                }
                continue;
            }            
            receiptRowsList.Add(receiptRow);
            //receiptRowsList.Add(receiptRow);
        }

    }

}
