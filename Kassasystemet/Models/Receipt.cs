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

        public Receipt()
        {
            var currentReceiptNumber = 1;
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
            ReceiptNumber = "#RN-" + currentReceiptNumber.ToString();
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
            var existingProduct = receiptRowsList.Where(p => p.ProductId == receiptRow.ProductId).FirstOrDefault();
            if (existingProduct != null)
                existingProduct.AddCount(receiptRow.Count);
            else
                receiptRowsList.Add(receiptRow);
        }
        public void ShowCurrentReceipt()
        {
            if (receiptRowsList.Count > 0)
            {
                foreach (var row in receiptRowsList)
                    Console.WriteLine($"{row.ProductName} {row.Count} * {row.Price} = {row.GetRowTotal()}");
                Console.WriteLine($"Total: {GetReceiptTotal()}");
            }
        }
        public void MakeProductFile()
        {
            foreach (var product in receiptRowsList)
            {
                var fileName = product.ProductName.ToString() + ".txt";
                if (!File.Exists(fileName))
                    using (var file = File.CreateText(fileName))
                        for (int i = 0; i < product.Count; i++)
                            file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd"));
                else
                    for (int i = 0; i < product.Count; i++)
                        File.AppendAllText(fileName, DateTime.Now.ToString("yyyy-MM-dd") + Environment.NewLine);
            }
        }

    }

}
