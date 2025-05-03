using Data.Data;
using Data.Interfaces;
using Data.Models;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class AppDbContextExtensions
{
    public static IServiceCollection AddAppDbContextServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {

       var connectionString = configuration.GetConnectionString("PostgresConnectionString"); 
       
       if (string.IsNullOrEmpty(connectionString))
       {
           throw new ArgumentException("SQL Server connection string is not configured.");
       }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}