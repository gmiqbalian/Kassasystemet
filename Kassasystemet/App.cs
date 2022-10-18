using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Kassasystemet.Models;

namespace Kassasystemet
{
    public class App
    {
        public void Run()
        {
            var productList = new List<Product>();
            productList = ReadProductsFromFile();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Välkommen till KASSA");
                Console.WriteLine("1. Ny kund");
                Console.WriteLine("2. Admin");
                Console.WriteLine("0. Avsluta");
                var input = Console.ReadLine();
                if (input == "1")
                    RegisterProduct(productList);
                else if (input == "2")
                    Admin(productList);
                else if (input == "0")
                    break;                
            }
        }

        private void Admin(List<Product> productList)
        {
            var newList = new List<Product>();
            newList = ReadProductsFromFile();
            Console.WriteLine("1. Ändra produkt namn");
            Console.WriteLine("2. Ändra produkt pris");
            Console.WriteLine("Ange val: ");

            var input = Console.ReadLine();
            if(input == "1")
            {
                Console.WriteLine("Ange produkt id för att byta namn: ");
                var toChange = Console.ReadLine();
                
                foreach (var product in newList)
                {
                    if(product.Name == toChange)
                    {
                        product.Name = toChange;
                    }
                    var line = $"{product.ProductId};{product.Name};{product.Price};{product.PriceType}";
                    File.WriteAllLines("Products.txt", line + Environment.NewLine);
                }
                using ()
                {

                }
                
                //foreach (var product in newList)
                //{
                //    if (product.ProductId == toChange)
                //    {
                //        Console.WriteLine("Ange nytt namn: ");
                //        var newName = Console.ReadLine();
                //        product.Name = newName;
                //    }
                //}
                //foreach (var product in newList)
                //{
                //    var rowLine = $"{product.ProductId};{product.Name};{product.Price};{product.PriceType}";
                    
                //}

            }
        }

        public void RegisterProduct(List<Product> productList)
        {
            Console.Clear();
            var newReceipt = new Receipt();
            Console.WriteLine("KASSA");
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"KVITTO {date}");
            Console.WriteLine($"kommandon: ");
            Console.WriteLine($"<product-id> <antal>");
            Console.WriteLine($"PAY");

            while (true)
            {
                Console.Write($"kommando: ");
                var salesInput = Console.ReadLine();
                if (salesInput.ToLower() == "pay")
                {                    
                    SaveReceipt(newReceipt);
                    break;
                }

                var split = salesInput.Split(' ');
                var productId = split[0];
                var productCount = int.Parse(split[1]);
                var soldProduct = MatchProductList(productList, productId); //product object
                if (soldProduct == null)
                    Console.WriteLine("Produkt med denna ID finns inte i lagar");                
                else
                {
                    var receiptRow = new ReceiptRow(soldProduct.ProductId, soldProduct.Name, productCount, soldProduct.Price);
                    newReceipt.AddToReceipt(receiptRow);
                }
                    
            }
        }
       
        public List<Product> ReadProductsFromFile()
        {
            var readResultList = new List<Product>();
            foreach (var line in File.ReadLines("Products.txt"))
            {
                var split = line.Split(';');
                var product = new Product(split[0], split[1], Convert.ToDecimal(split[2]), split[3]);
                
                readResultList.Add(product);
            }
            return readResultList;
        }
        public Product MatchProductList(List<Product> productList, string productId)
        {
            foreach (var product in productList)
            {
                if (product.ProductId == productId)
                    return product;
            }
            return null;
        }
        public void SaveReceipt(Receipt newReceipt)
        {
            
            var path = "RECEIPT_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            var receiptNumber = 100; //read from last number in file.
            File.AppendAllText(path, receiptNumber + Environment.NewLine);
            foreach (var row in newReceipt.receiptRowsList)
            {
                var rowText = $"{row.ProductName} {row.Count} * {row.Price} = {row.GetTotal()}";
                File.AppendAllText(path, rowText+ Environment.NewLine);

            }
            var total = $"Total: {newReceipt.GetTotal()}";
            File.AppendAllText(path, total + Environment.NewLine);
            receiptNumber+=1;
        }
    }
}
