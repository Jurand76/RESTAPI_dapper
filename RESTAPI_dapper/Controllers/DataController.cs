using Microsoft.AspNetCore.Mvc;
using RESTAPI_dapper.Services;


namespace RESTAPI_dapper.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost("loadData")]
        public IActionResult LoadData()
        {
            var csvContent = _dataService.CSVLoadData("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv");

            string filesPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            
            if (!Directory.Exists(filesPath))
            {
                Directory.CreateDirectory(filesPath);
            }

            string filePath = Path.Combine(filesPath, "Products.csv");
            System.IO.File.WriteAllText(filePath, csvContent);

            var filteredProducts = _dataService.ReadAndFilterProducts(filePath);
            _dataService.DeleteTableDetails("Products");
            _dataService.SaveProductsToDatabase(filteredProducts);

            csvContent = _dataService.CSVLoadData("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv");
            filePath = Path.Combine(filesPath, "Inventory.csv");
            System.IO.File.WriteAllText(filePath, csvContent);

            var filteredInventory = _dataService.ReadAndFilterInventory(filePath);
            _dataService.DeleteTableDetails("Inventory");
            _dataService.SaveInventoryToDatabase(filteredInventory);

            csvContent = _dataService.CSVLoadData("https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv");
            filePath = Path.Combine(filesPath, "Prices.csv");
            System.IO.File.WriteAllText(filePath, csvContent);

            var allPrices = _dataService.ReadPrices(filePath);
            _dataService.DeleteTableDetails("Prices");
            _dataService.SavePricesToDatabase(allPrices);

            return Ok();
        }

        [HttpGet("productDetails/{sku}")]
        public IActionResult GetProductDetails(string sku)
        {
            try
            {
                var productDetails = _dataService.GetProductDetailsBySku(sku);
                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

