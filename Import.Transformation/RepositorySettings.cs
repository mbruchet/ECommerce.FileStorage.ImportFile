namespace Import.Transformation
{
    public class RepositorySettings
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string ProviderType { get; set; }
        public ConnectionStringSettings ConnectionString { get; set; }
    }

    public class ConnectionStringSettings
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}