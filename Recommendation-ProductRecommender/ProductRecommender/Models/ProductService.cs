namespace ProductRecommender.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    public partial class ProductService
    {
        public readonly static int _productsToRecommend = 2;
        public readonly static int _trendingproducts = 20;
        public Lazy<List<Product>> _products = new Lazy<List<Product>>(() => LoadProductData());
        public List<Product> _trendingProducts = LoadTrendingProducts();
        public readonly static string _modelpath = @".\Models\model.zip";


        public static List<Product> LoadTrendingProducts()
        {

            var result = new List<Product>();

            result.Add(new Product { ProductID = 1573, ProductName = "Zucchini" });
            result.Add(new Product { ProductID = 1721, ProductName = "Pepper" });
            result.Add(new Product { ProductID = 49273, ProductName = "Mushroom" });
            result.Add(new Product { ProductID = 55816, ProductName = "Potato" });
            return result;
        }

        public string GetModelPath()
        {
            return _modelpath;
        }

        public IEnumerable<Product> GetSomeSuggestions()
        {
            var products = GetRecentProducts().ToArray();

            Random rnd = new Random();
            int[] productselector = new int[_productsToRecommend];
            for (int i = 0; i < _productsToRecommend; i++)
            {
                productselector[i] = rnd.Next(products.Length);
            }

            return productselector.Select(s => products[s]);
        }

        public IEnumerable<Product> GetRecentProducts()
        {
            return GetAllProducts()
                .Where(m => m.ProductName.Contains("5345")
                            || m.ProductName.Contains("23466")
                            || m.ProductName.Contains("21345"));
        }

        public Product Get(int id)
        {
            return _products.Value.Single(m => m.ProductID == id);
        }


        public IEnumerable<Product> GetAllProducts()
        {
            return _products.Value;
        }

        private static List<Product> LoadProductData()
        {
            var result = new List<Product>();

            Stream fileReader = File.OpenRead("Content/products.csv");

            StreamReader reader = new StreamReader(fileReader);
            try
            {
                bool header = true;
                int index = 0;
                var line = "";
                while (!reader.EndOfStream)
                {
                    if (header)
                    {
                        line = reader.ReadLine();
                        header = false;
                    }
                    line = reader.ReadLine();
                    string[] fields = line.Split(',');
                    int ProductID = Int32.Parse(fields[0].ToString().TrimStart(new char[] { '0' }));
                    string ProductName = fields[1].ToString();
                    result.Add(new Product() { ProductID = ProductID, ProductName = ProductName });
                    index++;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }

            return result;
        }
    }
}