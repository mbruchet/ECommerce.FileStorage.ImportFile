using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Import.Mapping.Product
{
    public class ProductModel
    {
        [Key]
        public long Id { get; set; }
        [JsonProperty("record_type")]
        public string RecordType { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("brand")]
        public string Brand { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("product_quantity")]
        public string ProductQuantity { get; set; }
        [JsonProperty("street_price")]
        public string StreetPrice { get; set; }
        [JsonProperty("suggested_price")]
        public string SuggestedPrice { get; set; }
        [JsonProperty("price_novat")]
        public string PriceNovat { get; set; }
        [JsonProperty("plain_description")]
        public string PlainDescription { get; set; }
        [JsonProperty("weight")]
        public string Weight { get; set; }
        [JsonProperty("picture1")]
        public string Picture1 { get; set; }
        [JsonProperty("picture2")]
        public string Picture2 { get; set; }
        [JsonProperty("picture3")]
        public string Picture3 { get; set; }
        public string Firme { get; set; }
        [JsonProperty("heel")]
        public string Heel { get; set; }
        public string Categorie { get; set; }
        public string Sottocategorie { get; set; }
        [JsonProperty("altro")]
        public string Altro { get; set; }
        [JsonProperty("season")]
        public string Season { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("partner")]
        public string Partner { get; set; }
        public string Warehouse2 { get; set; }
        public string Sunglasses { get; set; }
        public string Watches { get; set; }
        [JsonProperty("bicolors")]
        public string Bicolors { get; set; }
        public string Genere { get; set; }
        public string Print { get; set; }
        [JsonProperty("productname")]
        public string Productname { get; set; }
        [JsonProperty("model_id")]
        public string ModelId { get; set; }
        [JsonProperty("barcode")]
        public string Barcode { get; set; }
        [JsonProperty("model_size")]
        public string ModelSize { get; set; }
        [JsonProperty("model_quantity")]
        public string ModelQuantity { get; set; }
    }
}
