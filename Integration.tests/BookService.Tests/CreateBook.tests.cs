using System.Reflection;
using AutoFixture.Xunit2;
using FirstApi.DTO;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Integration.tests.BookService.Tests;

public class CreateBook_tests : IDisposable, IAsyncDisposable
{
    private readonly ITestOutputHelper output;
    private readonly ApplicationDbContext testDbContext;
    private readonly BooksService sut;

    public CreateBook_tests(ITestOutputHelper output)
    {
        this.output = output;

        var configuration =  new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly(),true).Build();
        
        var conStrBuilder = new SqlConnectionStringBuilder(
            configuration["ConnectionStrings:FirstApiIntegrationTests"])
        {
            Password = configuration["DbPassword"]
        };
        
        var connectionString = conStrBuilder.ConnectionString;
        testDbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options);
        sut = new BooksService(testDbContext);

    }
    
    [Theory]
    [AutoData]
    public async Task WhenCreateBook_BookSavedInDb(string title, string author, BookTypes type, int publicationYear)
    {
       var r =  await sut.CreateBookAsync(title, author, type, publicationYear);
        
        //Assert
        var newBook = testDbContext.Books.FindAsync(r);
        newBook.Should().NotBeNull();
    }


    public void Dispose()
    {
        //cleaning here for dbCOntext and db
        output.WriteLine("dispose is called");
    }

    public async ValueTask DisposeAsync()
    {
        //cleaning here for dbCOntext and db
        output.WriteLine("disposeAsync is called");

    }
}