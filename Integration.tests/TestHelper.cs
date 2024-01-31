using System.Reflection;
using AutoFixture;
using EFCore.BulkExtensions;
using FirstApi.Models;
using FirstApi.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Integration.tests;

public static class TestHelper
{
    private static readonly string ConnectionString = GetConnectionString();

   
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
            .UseSqlServer(ConnectionString)
            .Options);
    }
   public static void CleanDb(ApplicationDbContext testDbContext)
    {
        testDbContext.BulkDelete(testDbContext.Books);
        if (testDbContext.Books.ToList().Count != 0)
            throw new Exception("db clean up went wrong");
    } 
   
   public static T DeepCopy<T>(this T self)
   {
       var serialized = JsonConvert.SerializeObject(self);
       return JsonConvert.DeserializeObject<T>(serialized) ?? throw new Exception("could not create deep copy of object");
   }

   public static async Task<IEnumerable<Book>> CreateBooksInDbAsync(int count, Fixture fixture, ApplicationDbContext testDbContext)
   {
       var booksData = fixture.CreateMany<Book>(count).ToList();
       await testDbContext.BulkInsertAsync(booksData);
       var res = testDbContext.Books;
       Assert.Equal(count, res.Count());
       return res;
   }
}