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
            // Dalsza logika przetwarzania pobranych danych
            return Ok();
        }

        [HttpGet("productDetails/{sku}")]
        public void GetProductDetails(string sku)
        {
            // Tutaj wywołaj metodę serwisu do pobierania szczegółów produktu
            
        }
    }
}

