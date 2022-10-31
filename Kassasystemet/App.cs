using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
            Console.Clear();
            ShowProductList(productList);
            Console.Write("Ange produkt id för att byta namn eller pris: ");
            var input = Console.ReadLine();
            var productToChange = MatchProductList(productList, input);
            
            if (productToChange != null)
            {
                Console.Write("Ange nytt namn: ");
                var newName = Console.ReadLine();
                productToChange.Name = newName;

                Console.Write("Ange nytt pris: ");
                var newPrice = Console.ReadLine();
                productToChange.Price = Convert.ToDecimal(newPrice);
            }

            using (var file = File.CreateText("Products.txt"))
                foreach (var product in productList)
                    file.WriteLine($"{product.ProductId};{product.Name};{product.Price};{product.PriceType}");
        }
        public void ShowProductList(List<Product> productList)
        {
            foreach (var product in productList)            
                Console.WriteLine($"{product.ProductId} {product.Name} {product.PriceType} {product.Price}");            
        }
        public void RegisterProduct(List<Product> productList)
        {
            //Console.Clear();
            var newReceipt = new Receipt();

            while (true)
            {
                Console.Clear();

                ShowCurrentReceipt(newReceipt);
               
                var salesInput = Console.ReadLine();
                if (IsValidInput(salesInput))
                {
                    var split = salesInput.Split(' ');
                    var productId = split[0];
                    var productCount = int.Parse(split[1]);
                    var soldProduct = MatchProductList(productList, productId); //returns product object
                    if (soldProduct == null)
                        Console.WriteLine("Produkt med denna ID finns inte i lagar");
                    else
                    {
                        var receiptRow = new ReceiptRow(soldProduct.ProductId, soldProduct.Name, productCount, soldProduct.Price);
                        newReceipt.AddToReceipt(receiptRow);                        
                    }                    
                }
                if (salesInput.ToLower() == "pay") //control that it doesnt save empty receipt
                {
                    if (newReceipt.receiptRowsList.Count > 0)
                    {
                        SaveReceipt(newReceipt);
                        break;
                    }
                    else
                        Console.WriteLine("Lägg till produkter på kvitto");                
                }
            }
        }
        public bool IsValidInput(string salesInput)
        {
            if (salesInput.Length == 5 && salesInput.IndexOf(' ') == 3)
                return true;
            return false;
        }
        public void ShowCurrentReceipt(Receipt newReceipt)
        {
            Console.WriteLine("KASSA");
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"KVITTO {date}");
            if (newReceipt.receiptRowsList.Count > 0)
            {
                foreach (var row in newReceipt.receiptRowsList)
                    Console.WriteLine($"{row.ProductName} {row.Count} * {row.Price} = {row.GetRowTotal()}");
                Console.WriteLine($"Total: {newReceipt.GetReceiptTotal()}");
            }
            Console.WriteLine($"kommandon: ");
            Console.WriteLine($"<product-id> <antal>");
            Console.WriteLine($"PAY");
            Console.Write($"kommando: ");

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
            
            File.AppendAllText(path, newReceipt.ReceiptNumber + Environment.NewLine);
            foreach (var row in newReceipt.receiptRowsList)
            {
                var rowText = $"{row.ProductName} {row.Count} * {row.Price} = {row.GetRowTotal()}";
                File.AppendAllText(path, rowText + Environment.NewLine);
            }
            var total = $"Total: {newReceipt.GetReceiptTotal()}";
            File.AppendAllText(path, total + Environment.NewLine);

        }
    }
}
