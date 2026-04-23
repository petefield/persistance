namespace pefi.persistence;

public class MongoDbSettings
{
    public MongoDbSettings()
    {
    }

    public MongoDbSettings(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; set; } = string.Empty;
}
