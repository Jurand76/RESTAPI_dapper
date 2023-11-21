using RESTAPI_dapper.Models;

namespace RESTAPI_dapper.Services
{
    public interface IDataService
    {
        Product GetProductDetails(string sku);
        string CSVLoadData(string url);
        List<Product> ReadAndFilterProducts(string filePath);
        void SaveProductsToDatabase(IEnumerable<Product> products, string connectionString);
    }
}
