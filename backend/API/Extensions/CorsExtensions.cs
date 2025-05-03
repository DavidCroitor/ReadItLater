using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System; // For ArgumentNullException
using System.Linq; // For Linq methods

namespace API.Extensions;

public static class CorsExtensions
{
    // Consistent policy name
        public const string DefaultCorsPolicy = "_DefaultCorsPolicy";
        private const string CorsAllowedOriginsKey = "CorsSettings:AllowedOrigins"; // Example config key

        public static IServiceCollection AddConfiguredCors(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var allowedOriginsConfig = configuration.GetValue<string>(CorsAllowedOriginsKey);
            string[] origins = Array.Empty<string>();

            if (!string.IsNullOrWhiteSpace(allowedOriginsConfig))
            {
                origins = allowedOriginsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
            else
            {
                Console.WriteLine($"Warning: CORS AllowedOrigins ('{CorsAllowedOriginsKey}') not found or empty in configuration. No origins will be explicitly allowed by default policy.");
         }


            services.AddCors(options =>
            {
                options.AddPolicy(name: DefaultCorsPolicy,
                                  policy =>
                                  {
                                      if (origins.Any())
                                      {
                                          policy.WithOrigins(origins); // Allow specific origins
                                      }
                                      else
                                      {
                                          // No origins allowed by default (safer)
                                          Console.WriteLine("Warning: Default CORS Policy has no origins configured. Cross-origin requests may be blocked.");
                                      }

                                      policy.AllowAnyHeader()  // Allow any standard/custom header
                                            .AllowAnyMethod(); // Allow common HTTP methods (GET, POST, PUT, DELETE, etc.)

                                      // Use AllowCredentials carefully if your frontend needs to send cookies/auth headers
                                      // and ensure WithOrigins is used (cannot use AllowAnyOrigin with AllowCredentials)
                                      policy.AllowCredentials();
                                  });
            
            });

        return services;
    }
}
