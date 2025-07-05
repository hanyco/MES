namespace CodeGenerator.Application.Services;

public sealed class Settings
{
    public static Settings Default { get; private set; }
    public string ConnectionString { get; private init; }

    public static void Configure(string connectionString)
        => Default = new Settings { ConnectionString = connectionString };
}