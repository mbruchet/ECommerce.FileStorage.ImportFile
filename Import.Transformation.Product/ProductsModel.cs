using System.Collections.Generic;
using Newtonsoft.Json;

namespace Import.Mapping.Product
{
    public class ProductsModel
    {
        [JsonProperty("dropshipping-csv-example")]
        public IEnumerable<ProductModel> Products { get; set; }
    }
}