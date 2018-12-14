namespace ProductRecommender.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ProductRecommender.Models;
    using Newtonsoft.Json;
    using Microsoft.ML;
    using Microsoft.ML.Runtime.Api;
    using Microsoft.ML.Runtime.Data;
    using Microsoft.ML.Core.Data;
    using System.IO;
    using System.Linq;

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly ProfileService _profileService;
        private readonly AppSettings _appSettings;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger, IOptions<AppSettings> appSettings)
        {
            _productService = new ProductService();
            _profileService = new ProfileService();
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public ActionResult Choose()
        {
            return View(_productService.GetSomeSuggestions());
        }

        public ActionResult Recommend(int id)
        {
            Profile activeprofile = _profileService.GetProfileByID(id);

            // 1. Create the local environment
            var ctx = new MLContext();

            // 2. Load the ProductsRecommendation Model
            ITransformer loadedModel;
            using (var stream = new FileStream(_productService.GetModelPath(), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                loadedModel = ctx.Model.Load(stream);
            }

            // 3. Create a prediction function
            var predictionfunction = loadedModel.MakePredictionFunction<ProductEntry, Copurchase_prediction>(ctx);

            List<Tuple<int, float>> prob = new List<Tuple<int, float>>();
            List<int> BoughtProductsId = _profileService.GetProfileBoughtProducts(id);
            List<Product> BoughtProducts = new List<Product>();

            foreach (var prodId in BoughtProductsId)
            {
                BoughtProducts.Add(_productService.Get(prodId));
            }

            // 4. Create a Prediction Output Class
            Copurchase_prediction prediction = null;
            List<Tuple<Product, float>> suggestedProducts = new List<Tuple<Product, float>>();
            foreach (var boughtproduct in BoughtProducts)
            {
                foreach (var product in _productService.GetAllProducts())
                {
                    // 4. Call the Prediction for each product-pair
                    prediction = predictionfunction.Predict(new ProductEntry()
                    {
                        ProductID = (uint)boughtproduct.ProductID,
                        CoPurchaseProductID = (uint)product.ProductID
                    });

                    if (prediction.Score > 0)
                    {
                        // 5. Add the score for recommendation of each product in the product list
                        prob.Add(Tuple.Create(product.ProductID, prediction.Score));
                    }
                }

                foreach (var recommendedProds in prob.OrderByDescending(t => t.Item2).Take(20))
                {
                    suggestedProducts.Add(Tuple.Create(_productService.Get(recommendedProds.Item1), recommendedProds.Item2));
                }
            }


            // 6. Provide scores to the view to be displayed
            ViewData["boughtproducts"] = BoughtProducts;
            ViewData["suggestedproducts"] = suggestedProducts.OrderByDescending(t => t.Item2).GroupBy(i => i.Item1.ProductName).Select(i => i.First()).Take(3).ToList();

            return View(activeprofile);
        }
        
        public ActionResult Buy()
        {
            return View();
        }

        public ActionResult Profiles()
        {

            List<Profile> profiles = _profileService._profile;
            return View(profiles);
        }

        public ActionResult Bought(int id)
        {
            Profile activeprofile = _profileService.GetProfileByID(id);
            List<int> productIds = _profileService.GetProfileBoughtProducts(id);
            List<Product> BoughtProducts = new List<Product>();

            foreach (var prodId in productIds)
            {
                BoughtProducts.Add(_productService.Get(prodId));
            }
            ViewData["boughtproducts"] = BoughtProducts;
            ViewData["trendingproducts"] = _productService._trendingProducts;
            return View(activeprofile);
        }

        public class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            { }
        }

        public class Copurchase_prediction
        {
            public float Score { get; set; }
        }

        public class ProductEntry
        {
            [KeyType(Contiguous = true, Count = 262111, Min = 0)]
            public uint ProductID { get; set; }

            [KeyType(Contiguous = true, Count = 262111, Min = 0)]
            public uint CoPurchaseProductID { get; set; }
        }

    }

}
