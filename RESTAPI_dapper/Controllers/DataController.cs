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
            _dataService.SaveProductsToDatabase(filteredProducts);

            csvContent = _dataService.CSVLoadData("https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv");
            filePath = Path.Combine(filesPath, "Prices.csv");
            System.IO.File.WriteAllText(filePath, csvContent);

            csvContent = _dataService.CSVLoadData("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv");
            filePath = Path.Combine(filesPath, "Inventory.csv");
            System.IO.File.WriteAllText(filePath, csvContent);
                    
            return Ok();
        }

        [HttpGet("productDetails/{sku}")]
        public void GetProductDetails(string sku)
        {
            // Tutaj wywołaj metodę serwisu do pobierania szczegółów produktu
            
        }
    }
}

