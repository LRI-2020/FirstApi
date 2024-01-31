using System.Reflection;
using AutoFixture.Xunit2;
using EFCore.BulkExtensions;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using Xunit.Abstractions;

namespace Integration.tests.BookService.Tests;

public class CreateBookTests : IDisposable
{
    private readonly ApplicationDbContext testDbContext;
    private readonly BooksService sut;

    public CreateBookTests()
    {
        testDbContext = TestHelper.GetConfiguredTestDbContext();
        sut = new BooksService(testDbContext);
        TestHelper.CleanDb(testDbContext);

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
    
    [Theory]
    [AutoData]
    public async Task WhenCreateBookWithNullableProp_BookSavedWithDefaultValue(string title, string author)
    {
       var r =  await sut.CreateBookAsync(title, author, null, null);
        //Assert
        var newBook = await testDbContext.Books.FindAsync(r);
        newBook.Should().NotBeNull();
        newBook!.Type.Should().Be(BookTypes.Unknown);
        newBook.PublicationYear.Should().Be(1970);
    }

    public void Dispose()
    {
        testDbContext.Dispose();
    }
}