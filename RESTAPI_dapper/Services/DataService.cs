using RESTAPI_dapper.Models;
using System;

namespace RESTAPI_dapper.Services
{
    public class DataService : IDataService
    {
        private readonly IHttpClientFactory _clientFactory;

        public DataService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
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
                throw new Exception("Nie udało się pobrać pliku CSV.");
            }
        }

        public Product GetProductDetails(string sku)
        {
            var product = new Product();
            return product;
        }
    }
}
