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
           var origins = new List<string>();

            var allowedOriginsConfig = configuration.GetValue<string>(CorsAllowedOriginsKey);
            if (!string.IsNullOrWhiteSpace(allowedOriginsConfig))
            {
                origins.AddRange(allowedOriginsConfig.Split(',', 
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }
            
            // Always include localhost:3000 for development
            if (!origins.Contains("http://localhost:3000"))
            {
                origins.Add("http://localhost:3000");
                Console.WriteLine("Added http://localhost:3000 to CORS allowed origins");
            }


            services.AddCors(options =>
            {
                options.AddPolicy(name: DefaultCorsPolicy,
                                  policy =>
                                  {
                                      if (origins.Any())
                                      {
                                          policy.WithOrigins(origins.ToArray())
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials();
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
