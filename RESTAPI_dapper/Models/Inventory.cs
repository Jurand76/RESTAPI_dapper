using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Hosting;
using RESTAPI_dapper.Models;
using System;

namespace RESTAPI_dapper.Models
{
    public class Inventory
    {
        public int ID { get; set; }
        public int Product_ID { get; set; }
        public string SKU { get; set; }
        public string Unit { get; set; }
        public int Qty { get; set; }
        public string Manufacturer { get; set; }
        public string Shipping_Time { get; set; }
        public decimal Shipping_Cost { get; set; }
    }
}
