using Microsoft.AspNetCore.Mvc;
using RESTAPI_dapper.Models;

namespace RESTAPI_dapper.Services
{
    public interface IDataService
    {
        Product GetProductDetails(string sku);
        string CSVLoadData(string url);
        void DeleteTableDetails(string tableName);
        List<Product> ReadAndFilterProducts(string filePath);
        void SaveProductsToDatabase(IEnumerable<Product> products);
        List<Inventory> ReadAndFilterInventory(string filePath);
        void SaveInventoryToDatabase(IEnumerable<Inventory> products);
        List<Prices> ReadPrices(string filePath);
        void SavePricesToDatabase(IEnumerable<Prices> prices);
        string GetProductDetailsBySku(string sku);
    }
}
