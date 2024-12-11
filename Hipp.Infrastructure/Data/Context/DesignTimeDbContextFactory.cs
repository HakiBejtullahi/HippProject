using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hipp.Infrastructure.Data.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Get the API project directory (two levels up from Infrastructure)
        var currentDir = Directory.GetCurrentDirectory();
        var apiProjectDir = Path.GetFullPath(Path.Combine(currentDir, "..", "Hipp.API"));
        var envPath = Path.Combine(apiProjectDir, ".env");

        if (File.Exists(envPath))
        {
            foreach (var line in File.ReadAllLines(envPath))
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    Environment.SetEnvironmentVariable(parts[0], parts[1]);
                }
            }
        }

        var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"};" +
                             $"Database={Environment.GetEnvironmentVariable("DB_NAME") ?? "HippDb"};" +
                             $"User={Environment.GetEnvironmentVariable("DB_USER") ?? "root"};" +
                             $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseMySql(
            connectionString, 
            ServerVersion.AutoDetect(connectionString),
            x => x.MigrationsAssembly("Hipp.Infrastructure")
        );

        return new ApplicationDbContext(builder.Options);
    }
} 