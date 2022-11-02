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
                Console.Write("Ange ditt val: ");
                var input = 0;
                var validInput = int.TryParse(Console.ReadLine(), out input);
                
                if(validInput && input >= 0 && input <= 2)
                {
                    if (input == 1)
                        RegisterProduct(productList);
                    else if (input == 2)
                    {
                        Console.WriteLine("1. Byta namn eller pris");
                        Console.WriteLine("2. Visa produktstatistik");
                        var adminInput = Console.ReadLine();
                        if (adminInput == "1")
                            Admin(productList);
                        else if (adminInput == "2")
                            GetProductStat(productList);                            
                    }
                    else if (input == 0)
                        break;
                }
                else
                {
                    Console.WriteLine("Vänlingen mata in rätt inmatning");
                    Console.ReadLine();
                }
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
                    file.WriteLine($"{product.ProductId};{product.Name};{product.Price};{product.PriceType};{product.Stock}");
        }
        public void ShowProductList(List<Product> productList)
        {
            foreach (var product in productList)            
                Console.WriteLine($"{product.ProductId} {product.Name} {product.PriceType} {product.Price}");            
        }
        public void RegisterProduct(List<Product> productList)
        {
            Console.Clear();
            var newReceipt = new Receipt();

            while (true)
            {
                Console.Clear();

                ShowCurrentReceipt(newReceipt);
               
                var salesInput = Console.ReadLine();
                if (salesInput.ToLower() != "pay" && IsValidInput(salesInput))
                {
                    var split = salesInput.Split(' ');
                    var productId = split[0];
                    var productCount = int.Parse(split[1]);
                    var soldProduct = MatchProductList(productList, productId); //returns product object
                    if (soldProduct == null)
                    {
                        Console.WriteLine("Produkt med denna ID finns inte i lagar");
                        Console.ReadLine();
                    }
                    else
                    {
                        var receiptRow = new ReceiptRow(soldProduct.ProductId, soldProduct.Name, productCount, soldProduct.Price);
                        AdjustStock(productList, productCount, soldProduct);
                        newReceipt.AddToReceipt(receiptRow);      
                    }
                }
                else if (salesInput.ToLower() == "pay") //control that it doesnt save empty receipt
                {
                    if (newReceipt.receiptRowsList.Count > 0)
                    {
                        MakeProductFile(newReceipt);
                        SaveReceipt(newReceipt);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Lägga till några produkter på kvitto");
                        Console.ReadLine();
                    }
                    
                }
            }
        }
        public void AdjustStock(List<Product> productList, int productCount, Product soldProduct)
        {
            soldProduct.Stock -= productCount;
            using (var file = File.CreateText("Products.txt"))
                foreach (var product in productList)
                    file.WriteLine($"{product.ProductId};{product.Name};{product.Price};{product.PriceType};{product.Stock}");
        }
        public void GetProductStat(List<Product> productList) // try solving with LINQ
        {
            Console.Clear();
            Console.Write("Ange product Id: ");
            var productId = Console.ReadLine();
            Console.WriteLine("Ange start datum (exempel 2022-01-01): ");
            var startDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Ange slut datum (exempel 2022-01-01): ");
            var endDate = DateTime.Parse(Console.ReadLine());

            var product = MatchProductList(productList, productId);
            var sum = 0;
            if(product!= null)
            {
                var file = product.Name.ToString() + ".txt";
                foreach (var line in File.ReadLines(file))
                {
                    var d = DateTime.Parse(line);
                    if(d >= startDate && d <= endDate)
                        sum++;                   
                }               
            }
            Console.WriteLine($"{product.Name}: {sum}");
            Console.ReadLine();
        }
        public void MakeProductFile(Receipt newReceipt)
        {
            foreach (var product in newReceipt.receiptRowsList)
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
        public bool IsValidInput(string salesInput)
        {
            var split = salesInput.Split(' ');
            if (split[0].Length == 0 || split[1].Length == 0)
                return false;

            var productId = 0;
            var productCount = 0;

            int.TryParse(split[0], out productId);
            int.TryParse(split[1], out productCount);

            if (productId > 0 && productCount > 0 && salesInput.IndexOf(' ') == 3)
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
                var product = new Product(split[0], split[1], Convert.ToDecimal(split[2]), split[3], Convert.ToInt32(split[4]));
                
                readResultList.Add(product);
            }
            return readResultList;
        }
        public Product MatchProductList(List<Product> productList, string productId)
        {
            foreach (var product in productList)            
                if (product.ProductId == productId)
                    return product;            
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
