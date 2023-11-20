using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;

namespace RESTAPI_dapper.Models
{
    public class Prices
    {
        public int ID { get; set; }
        public string SKU { get; set; }
        public decimal Net_Price { get; set; }
        public decimal Discounted_Price { get; set; }
        public decimal Logistic_Unit_Price { get; set; }
    }
}
