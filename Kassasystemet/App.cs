using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassasystemet
{
    public class App
    {
        public void Run()
        {
            var productList = new List<Products>();
            productList = ReadProductsFromFile();
        }

        public List<Products> ReadProductsFromFile()
        {
            var readResult = new List<Products>();
            foreach (var line in File.ReadLines("Products.txt"))
            {
                var split = line.Split(';');
                var product = new Products()
                {
                    Code = Convert.ToInt32(split[0]),
                    Name = split[1],
                    Price = int.Parse(split[2]),
                    Unit = split[3]
                };
            }
            return readResult;
        }
    }
}
