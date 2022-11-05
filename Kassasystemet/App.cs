using Kassasystemet.Models;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Reflection.Metadata;

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
                ShowMainMenu();
                var input = 0;
                var validInput = int.TryParse(Console.ReadLine(), out input);

                if (validInput && input >= 0 && input <= 2)
                {
                    if (input == 1)
                        RegisterProduct(productList);
                    else if (input == 2)
                    {
                        Console.WriteLine("1. Byta namn eller pris");
                        Console.WriteLine("2. Försäljningsstatistik");
                        Console.Write("Ange ditt val: ");
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
        public void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Välkommen till KASSA");
            Console.WriteLine("1. Ny kund");
            Console.WriteLine("2. Admin");
            Console.WriteLine("0. Avsluta");
            Console.Write("Ange ditt val: ");
        }
        public void RegisterProduct(List<Product> productList)
        {
            Console.Clear();
            var newReceipt = new Receipt();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("KASSA");
                var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine($"KVITTO {date}");
                newReceipt.ShowCurrentReceipt();
                Console.WriteLine($"kommandon: ");
                Console.WriteLine($"<product-id> <antal>");
                Console.WriteLine($"PAY");
                Console.Write($"kommando: ");

                var salesInput = Console.ReadLine();

                if (!salesInput.Any(char.IsLetter) && IsValidSaleCommand(salesInput))
                {
                    var split = salesInput.Split(' ');
                    var productId = split[0];
                    var productCount = int.Parse(split[1]);
                    var soldProduct = MatchProductList(productList, productId);
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
                else if (IsPayCommand(salesInput)) //controls that it doesnt save empty receipt
                {
                    if (newReceipt.receiptRowsList.Count > 0)
                    {
                        newReceipt.MakeProductFile();
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
        public void AdjustStock(List<Product> productList, int productCount, Product soldProduct)
        {
            soldProduct.Stock -= productCount;
            using (var file = File.CreateText("Products.txt"))
                foreach (var product in productList)
                    file.WriteLine($"{product.ProductId};{product.Name};{product.Price};{product.PriceType};{product.Stock}");
        }
        public void GetProductStat(List<Product> productList)
        {
            Console.Clear();
            Console.Write("Ange start datum (exempel 2022-01-01): ");
            var startDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Ange slut datum (exempel 2022-01-01): ");
            var endDate = DateTime.Parse(Console.ReadLine());

            var report = new Dictionary<string, int>();

            foreach (var product in productList)
            {
                var path = product.Name.ToString() + ".txt";
                if (File.Exists(path))
                {
                    var sum = 0;
                    foreach (var line in File.ReadLines(path))
                    {
                        DateTime.TryParse(line, out DateTime selectedDate);
                        if (selectedDate >= startDate && selectedDate <= endDate)                        
                            sum++;                        
                    }
                    report.Add(product.Name, sum);
                }
            }

            report
                .OrderByDescending(r => r.Value)
                .ToList()
                .ForEach(r => Console.WriteLine($"{r.Key}: {r.Value}"));

            Console.ReadLine();
        }
        public bool IsValidSaleCommand(string salesInput)
        {
            if (salesInput.Length >= 5 && salesInput.IndexOf(' ') == 3)
            {
                var split = salesInput.Split(' ');

                int.TryParse(split[0], out var productId);
                int.TryParse(split[1], out var productCount);

                if (productId > 0 && productCount > 0)
                    return true;
                else
                {
                    Console.WriteLine("Ange rätt kommando");
                    Console.ReadLine();
                }
            }
            return false;
        }
        public bool IsPayCommand(string salesInput)
        {
            if (salesInput.All(Char.IsLetter) && salesInput.ToLower() == "pay")
                return true;
            return false;
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
