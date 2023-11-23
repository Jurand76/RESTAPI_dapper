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


        // endpoint nr 1 - ładowanie danych z plików CSV, zapis na dysku lokalnym, wrzucenie danych do bazy do tabel Products, Inventory, Prices

        [HttpPost("loadData")]
        public IActionResult LoadData()
        {
            // pobieranie pliku Products.csv
            
            var csvContent = _dataService.CSVLoadData("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv");
            string filesPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            
            if (!Directory.Exists(filesPath))
            {
                Directory.CreateDirectory(filesPath);
            }

            string filePath = Path.Combine(filesPath, "Products.csv");
            System.IO.File.WriteAllText(filePath, csvContent);


            // filtrowanie wyników, czyszczenie tabeli w bazie, zapis nowych danych do bazy

            var filteredProducts = _dataService.ReadAndFilterProducts(filePath);
            _dataService.DeleteTableDetails("Products");
            _dataService.SaveProductsToDatabase(filteredProducts);


            // pobieranie pliku Inventory.csv

            csvContent = _dataService.CSVLoadData("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv");
            filePath = Path.Combine(filesPath, "Inventory.csv");
            System.IO.File.WriteAllText(filePath, csvContent);


            // filtrowanie wyników, czyszczenie tabeli w bazie, zapis nowych danych do bazy

            var filteredInventory = _dataService.ReadAndFilterInventory(filePath);
            _dataService.DeleteTableDetails("Inventory");
            _dataService.SaveInventoryToDatabase(filteredInventory);


            // pobieranie pliku Prices.csv

            csvContent = _dataService.CSVLoadData("https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv");
            filePath = Path.Combine(filesPath, "Prices.csv");
            System.IO.File.WriteAllText(filePath, csvContent);


            // czyszczenie tabeli w bazie, zapis nowych danych do bazy

            var allPrices = _dataService.ReadPrices(filePath);
            _dataService.DeleteTableDetails("Prices");
            _dataService.SavePricesToDatabase(allPrices);

            return Ok();
        }


        // endpoint nr 2 - odczyt danych z bazy na podstawie numeru SKU produktu

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

