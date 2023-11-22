using CsvHelper.Configuration;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;

namespace RESTAPI_dapper.Models
{
    public class Prices
    {
        public string SKU { get; set; }
        public string Logistic_Unit_Price { get; set; }
    }
}
