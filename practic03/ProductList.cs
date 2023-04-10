using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace practic03
{
    public class ProductList
    {
        private readonly string _path;
        private Dictionary<string, Product[]> _folderContent = new Dictionary<string, Product[]>();

        private List<Product> _products
        {
            get
            {
                var products = new List<Product>();

                foreach (var product in _folderContent.Values)
                {
                    products.AddRange(product);
                }

                return products;
            }
        }

        public ProductList(string path)
        {
            _path = path;
            Load();
        }

        public static Action<Product> PrintProduct = (product) =>
        {
            Console.WriteLine($"{product.name}\n{product.quantity}\n{product.dateTime}");
        };

        public Product[] GetProductsForDate(DateTime dateTime)
        {
            var predicate = CreateDateTimePredicate(dateTime);

            return _products.Where(product => predicate(product)).ToArray();
        }

        private Predicate<Product> CreateDateTimePredicate(DateTime dateTime)
        {
            Func<DateTime, DateTime, bool> CompareDate = (dateTime1, dateTime2) =>
                dateTime1.Day == dateTime2.Day && dateTime1.Month == dateTime2.Month && dateTime1.Year == dateTime2.Year;

            return (product) => CompareDate(product.dateTime, dateTime);
        }

        private void Load()
        {
            if (!Directory.Exists(_path))
            {
                return;
            }

            var fileNames = Directory.GetFiles(_path);

            var jsonFiles = fileNames.ToList().Where((fileName) => fileName.EndsWith(".json"));

            foreach (var jsonFilePath in jsonFiles)
            {
                var content = File.ReadAllText(jsonFilePath);

                try
                {
                    var products = JsonConvert.DeserializeObject<Product[]>(content);

                    if (products != null)
                    {
                        _folderContent[jsonFilePath] = products;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Can't parse {jsonFilePath} file, may it is corrupted or don't match schema");
                }
            }
        }
    }
}
