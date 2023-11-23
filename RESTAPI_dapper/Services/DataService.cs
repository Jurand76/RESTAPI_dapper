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

        // Ładowanie danych z pliku CSV
        public string CSVLoadData(string url)
        {
            var httpClient = _clientFactory.CreateClient();
            var response = httpClient.GetAsync(url).Result;  

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;  
                return content;
            }
            else
            {
                throw new Exception($"Nie udało się pobrać pliku CSV z url: {url}");
            }
        }


        // Usuwanie zawartości tabeli bazy danych - parametr nazwa tabeli. 
        public void DeleteTableDetails(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sql = $"DELETE FROM {tableName}";
            connection.Execute(sql);
            connection.Close();
            _logger.LogInformation($"All products from table {tableName} deleted!");
        }


        // Odczyt pliku z produktami, filtrowanie wg wytycznych - produkty wysyłane w przeciągu 24h
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
                    if (product.Shipping.Contains("24h") && !product.Is_Wire)
                    {
                        products.Add(product);
                    }
                }
                catch 
                {
                    Console.WriteLine("Reading error in products.csv");
                }
            }

            return products;
        }


        // Zapis listy produktów do bazy danych
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

       
        // Odczyt pliku ze stanami magazynowymi, filtrowanie wg wytycznych - produkty, które są wysyłane w przeciągu 24h
        public List<Inventory> ReadAndFilterInventory(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                IgnoreBlankLines = true,
                MissingFieldFound = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<InventoryMap>();

            var products = new List<Inventory>();

            while (csv.Read())
            {
                try
                {
                    var product = new Inventory
                    {
                        Product_ID = csv.GetField<int>(0),
                        SKU = csv.GetField<string>(1),
                        Unit = csv.GetField<string>(2),
                        Qty = csv.GetField<decimal>(3),
                        Manufacturer = csv.GetField<string>(4),
                        Shipping = csv.GetField<string>(6),
                    };

                    // Attempt to get the Shipping_Cost field
                    if (!csv.TryGetField<decimal>(7, out var shippingCost))
                    {
                        shippingCost = 0;  // Set to 0 if the field is missing or can't be parsed
                    }
                    product.Shipping_Cost = shippingCost;

                    if (product.Shipping.Contains("24h"))
                    {
                        products.Add(product);
                    }
                }
                catch
                {
                    Console.WriteLine("Reading error in inventory.csv");
                }
            }

            return products;
        }


        // Zapis magazynu do bazy danych
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


        // Odczyt tabeli z cenami produktów
        public List<Prices> ReadPrices(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                IgnoreBlankLines = true,
                MissingFieldFound = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
           
            var prices = new List<Prices>();

            while (csv.Read())
            {
                try
                {
                    var price = new Prices
                    {
                        SKU = csv.GetField<string>(1),
                        Logistic_Unit_Price = csv.GetField<string>(5)
                    };

                    prices.Add(price);
                    
                }
                catch
                {
                    Console.WriteLine("Reading error in inventory.csv");
                }
            }

            return prices;
        }


        // Zapis tabeli z cenami produktów do bazy danych
        public void SavePricesToDatabase(IEnumerable<Prices> prices)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            foreach (var price in prices)
            {
                var sql = "INSERT INTO Prices (SKU, Logistic_Unit_Price) VALUES (@SKU, @Logistic_Unit_Price)";
                connection.Execute(sql, price);
            }
            connection.Close();
            _logger.LogInformation("Table Prices has been created!");
        }


        // Odczyt danych dotyczących produktu o określonym SKU, wg założenia zadania
        public string GetProductDetailsBySku(string sku)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT 
                    p.Name,
                    p.EAN,
                    p.Producer_name,
                    p.Category,
                    p.Default_Image,
                    i.Qty as Stock,
                    i.Unit,
                    pr.Logistic_Unit_Price as NetPrice,
                    i.Shipping_Cost
                FROM 
                    Products p
                JOIN 
                    Inventory i ON p.SKU = i.SKU
                JOIN 
                    Prices pr ON p.SKU = pr.SKU
                WHERE 
                    p.SKU = @sku;";

            var result = connection.QueryFirstOrDefault(query, new { sku });

            if (result == null)
                return "Product not found";                     // Jeśli produkt nie został znaleziony, wysyłany jest stosowny komunikat

            // String wysyłany do endpointu
            return $"{result.SKU} {result.Name} {result.EAN} {result.Producer_name}, Category: {result.Category} Stock: {result.Stock}, NetPrice: {result.NetPrice} for {result.Unit}, ShippingCost: {result.Shipping_Cost}, Image: {result.Default_Image}";
        }
    }
}

