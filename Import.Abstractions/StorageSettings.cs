namespace Import.Abstractions
{
    public class AzureBlobSettings
    {
        public string StorageAccount { get; set; }
        public string StorageKey { get; set; }
        public bool UseHttps { get; set; }
        public string ShareName { get; set; }
    }
}