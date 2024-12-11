using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hipp.Common.Configuration;


    public static class EnvironmentConfiguration
    {
        public static string GetConnectionString()
        {
            return $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                   $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                   $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                   $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";
        }

        public static string GetJwtSecret()
        {
            return Environment.GetEnvironmentVariable("JWT_SECRET");
        }

        public static string GetJwtIssuer()
        {
            return Environment.GetEnvironmentVariable("JWT_ISSUER");
        }

        public static string GetJwtAudience()
        {
            return Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        }

        public static int GetJwtExpirationMinutes()
        {
            var minutes = Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES");
            return int.TryParse(minutes, out int result) ? result : 60; // default to 60 if not set
        }
    }
