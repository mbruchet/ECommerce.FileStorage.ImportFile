using System.Collections.Generic;
using Import.Abstractions.Interfaces;
using Newtonsoft.Json;

namespace Import.Mapping.Product
{
    public class DataMapper:IDataMapper<ProductModel>
    {
        public IEnumerable<ProductModel> MapData(string json)
        {
            var data = JsonConvert.DeserializeObject<ProductsRootModel>(json).Products.Products;
            return data;
        }
    }
}
