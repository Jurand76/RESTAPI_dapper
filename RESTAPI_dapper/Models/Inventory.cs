using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Hosting;
using RESTAPI_dapper.Models;
using System;

namespace RESTAPI_dapper.Models
{
    public class Inventory
    {
        public int Product_ID { get; set; }
        public string SKU { get; set; }
        public string Unit { get; set; }
        public decimal Qty { get; set; }
        public string Manufacturer { get; set; }
        public string Shipping { get; set; }
        public decimal Shipping_Cost { get; set; }
    }

    public class InventoryMap : ClassMap<Inventory>
    {
        public InventoryMap()
        {
            Map(m => m.Product_ID).Name("product_id");
            Map(m => m.SKU).Name("sku");
            Map(m => m.Unit).Name("unit");
            Map(m => m.Qty).Name("qty");
            Map(m => m.Manufacturer).Name("manufacturer_name");
            Map(m => m.Shipping).Name("shipping");
            Map(m => m.Shipping_Cost).Name("shipping_cost");
        }
    }
}
