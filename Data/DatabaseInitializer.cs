using System.Data.SqlClient;

namespace DynamicFormBuilder.Data
{
    public class DatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connString))
            {
                _logger.LogWarning("No DefaultConnection found in configuration.");
                return;
            }

            try
            {
                var builder = new SqlConnectionStringBuilder(connString);
                // ensure we connect to master to create database if needed
                builder.InitialCatalog = "master";
                var masterConn = builder.ConnectionString;

                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "DynamicFormBuilderDB.sql");
                if (!File.Exists(scriptPath))
                {
                    _logger.LogWarning("Database script not found at {Path}", scriptPath);
                    return;
                }

                var script = await File.ReadAllTextAsync(scriptPath);
                var batches = script.Split(new[] { "\nGO\n", "\r\nGO\r\n", "\nGO\r\n", "\r\nGO\n" }, StringSplitOptions.RemoveEmptyEntries);

                using (var connection = new SqlConnection(masterConn))
                {
                    await connection.OpenAsync();

                    foreach (var batch in batches)
                    {
                        var trimmed = batch.Trim();
                        if (string.IsNullOrWhiteSpace(trimmed))
                            continue;

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandTimeout = 600;
                            command.CommandText = trimmed;
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }

                _logger.LogInformation("Database initialization script executed (if needed).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize database. Please run Database/DynamicFormBuilderDB.sql manually and ensure the connection string has sufficient permissions.");
            }
        }
    }
}
