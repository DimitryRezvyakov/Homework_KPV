using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UserRegistration.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext> 
{ 
    public AppDbContext CreateDbContext(string[] args)
    { 
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString)) 
        { 
            throw new InvalidOperationException("Connection string 'DefaultConnection' не найдена."); 
        } 

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new AppDbContext(optionsBuilder.Options);
    } 
}

