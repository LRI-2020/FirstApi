using System.Reflection;
using EFCore.BulkExtensions;
using FirstApi.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Integration.tests;

public static class TestHelper
{
    public static readonly string connectionString = GetConnectionString();

   
    static string GetConnectionString()
    {
        var configuration =  new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly(),true).Build();
        var conStrBuilder = new SqlConnectionStringBuilder(
            configuration["ConnectionStrings:FirstApiIntegrationTests"])
        {
            Password = configuration["DbPassword"]
        };
        return conStrBuilder.ConnectionString;
        
    }

    public static ApplicationDbContext GetConfiguredTestDbContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options);
    }
   public static void CleanDb(ApplicationDbContext testDbContext)
    {
        testDbContext.BulkDelete(testDbContext.Books);
        if (testDbContext.Books.ToList().Count != 0)
            throw new Exception("db clean up went wrong");
    } 
}