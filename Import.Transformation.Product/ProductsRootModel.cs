using Newtonsoft.Json;

namespace Import.Mapping.Product
{
    public class ProductsRootModel
    {
        [JsonProperty("dropshipping-csv-example")]
        public ProductsModel Products { get; set; }
    }
}
