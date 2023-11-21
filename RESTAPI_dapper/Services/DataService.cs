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

        public DataService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

        public class ProductMap : ClassMap<Product>
        {
            public ProductMap()
            {
                Map(m => m.ID).Name("ID");
                Map(m => m.SKU).Name("SKU");
                Map(m => m.Name).Name("name");
                Map(m => m.EAN).Name("EAN");
                Map(m => m.Producer_name).Name("producer_name");
                Map(m => m.Category).Name("category");
                Map(m => m.Is_Wire).Name("is_wire");
                Map(m => m.Shipping).Name("shipping");
                Map(m => m.Available).Name("available");
                Map(m => m.Is_Vendor).Name("is_vendor");
                Map(m => m.Default_Image).Name("default_image");
            }
        }

        public List<Product> ReadAndFilterProducts(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                IgnoreBlankLines = true,
                ShouldSkipRecord = (records) =>
                {
                    Console.WriteLine("Error: ", records)
                    return false;
                }
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<ProductMap>();
            return csv.GetRecords<Product>().Where(p => !p.Is_Wire && p.Shipping == "24h").ToList();
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
        }
    }
}
