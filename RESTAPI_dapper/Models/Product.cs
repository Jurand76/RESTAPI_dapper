using CsvHelper.Configuration;

namespace RESTAPI_dapper.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string EAN { get; set; }
        public string Producer_name { get; set; }
        public string Category { get; set; }
        public bool Is_Wire { get; set; }
        public string Shipping { get; set; }
        public bool Available { get; set; }
        public bool Is_Vendor { get; set; }
        public string Default_Image { get; set; }
    }

    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Map(m => m.ID).Name("ID");
            Map(m => m.SKU).Name("SKU");
            Map(m => m.Name).Name("name");
            Map(m => m.EAN).Name("EAN");
            Map(m => m.Producer_name).Name("producer_name");
            Map(m => m.Category).Name("category");
            Map(m => m.Is_Wire).Name("is_wire");
            Map(m => m.Shipping).Name("shipping");
            Map(m => m.Available).Name("available");
            Map(m => m.Is_Vendor).Name("is_vendor");
            Map(m => m.Default_Image).Name("default_image");
        }
    }
}
