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
        public string ReceiptNumber { get; }
        public List<ReceiptRow> receiptRowsList = new List<ReceiptRow>();
        private static int receiptNumberSeed = 100;
        public Receipt()
        {
            var currentReceiptNumber = 0;
            var path = "RECEIPT_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            if (File.Exists(path))
            {
                foreach (var line in File.ReadLines(path))
                {
                    if (line.Contains("#RN-"))
                    {
                        int.TryParse(line.Split('-')[1], out currentReceiptNumber);
                        currentReceiptNumber++;
                    }
                }
            }
            var receiptNumberBuilder = new StringBuilder();
            receiptNumberBuilder.Append("#RN-");
            if (currentReceiptNumber > 0)
                receiptNumberBuilder.Append(currentReceiptNumber);
            else
                receiptNumberBuilder.Append(receiptNumberSeed);

            ReceiptNumber = receiptNumberBuilder.ToString();
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
        public void AddToReceipt(ReceiptRow receiptRow)
        {
            var row = receiptRowsList.Where(r => r.ProductId == receiptRow.ProductId).FirstOrDefault();
            if (row != null)
            {
                row.AddCount(receiptRow.Count);
                return;
            }            
            receiptRowsList.Add(receiptRow);            
        }

    }

}
