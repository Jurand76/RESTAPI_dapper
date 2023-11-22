using RESTAPI_dapper.Models;
using System;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace RESTAPI_dapper.Services
{
    public class DataService : IDataService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _connectionString;
        private readonly ILogger<DataService> _logger;

        public DataService(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<DataService> logger)
        {
            _clientFactory = clientFactory;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public string CSVLoadData(string url)
        {
            var httpClient = _clientFactory.CreateClient();
            var response = httpClient.GetAsync(url).Result;  // Używamy .Result do synchronicznego pobrania wyniku

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;  // Ponownie, używamy .Result
                return content;
            }
            else
            {
                throw new Exception("Nie udało się pobrać plików CSV.");
            }
        }

        public Product GetProductDetails(string sku)
        {
            var product = new Product();
            return product;
        }

        
        public List<Product> ReadAndFilterProducts(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                IgnoreBlankLines = true,
            
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<ProductMap>();

            var products = new List<Product>();

            while (csv.Read())
            {
                try
                {
                    var product = csv.GetRecord<Product>();
                    if (product.Shipping == "24h" && !product.Is_Wire)
                    {
                        products.Add(product);
                    }
                }
                catch 
                {
                    Console.WriteLine("Reading products.csv stopped");
                }
            }

            return products;
        }

        public void DeleteTableDetails(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sql = $"DELETE FROM {tableName}";
            connection.Execute(sql);
            connection.Close();
            _logger.LogInformation($"All products from table {tableName} deleted!");
        }
        public void SaveProductsToDatabase(IEnumerable<Product> products)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            foreach (var product in products)
            {
                var sql = "INSERT INTO Products (ID, SKU, Name, EAN, Producer_name, Category, Is_Wire, Available, Is_Vendor, Default_Image) VALUES (@ID, @SKU, @Name, @EAN, @Producer_name, @Category, @Is_Wire, @Available, @Is_Vendor, @Default_Image)";
                connection.Execute(sql, product);
            }
            connection.Close();
            _logger.LogInformation("Table Products has been created!");
        }

       
        public List<Inventory> ReadAndFilterInventory(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                IgnoreBlankLines = true,

            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<InventoryMap>();

            var products = new List<Inventory>();

            while (csv.Read())
            {
                try
                {
                    var product = csv.GetRecord<Inventory>();
                    if (product.Shipping == "24h")
                    {
                        products.Add(product);
                    }
                }
                catch
                {
                    Console.WriteLine("Reading inventory.csv stopped");
                }
            }

            return products;
        }
                
        public void SaveInventoryToDatabase(IEnumerable<Inventory> products)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            foreach (var product in products)
            {
                var sql = "INSERT INTO Inventory (Product_ID, SKU, Unit, Qty, Manufacturer, Shipping, Shipping_Cost) VALUES (@Product_ID, @SKU, @Unit, @Qty, @Manufacturer, @Shipping, @Shipping_Cost)";
                connection.Execute(sql, product);
            }
            connection.Close();
            _logger.LogInformation("Table Inventory has been created!");
        }
    }
}

