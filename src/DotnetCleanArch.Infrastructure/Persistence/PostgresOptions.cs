namespace DotnetCleanArch.Infrastructure.Persistence;

public sealed class PostgresOptions
{
    public const string SectionName = "Postgres";

    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 5432;

    public string Database { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IncludeErrorDetail { get; set; }

    public string BuildConnectionString() =>
        $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};Include Error Detail={IncludeErrorDetail}";
}
