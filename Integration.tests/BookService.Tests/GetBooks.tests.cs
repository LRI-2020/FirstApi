using AutoFixture;
using EFCore.BulkExtensions;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;

namespace Integration.tests.BookService.Tests;

public class GetBooksTests
{
    private readonly ApplicationDbContext testDbContext;
    private readonly BooksService sut;

    public GetBooksTests()
    {
        testDbContext = TestHelper.GetConfiguredTestDbContext();
        sut = new BooksService(testDbContext);
        TestHelper.CleanDb(testDbContext);

    }

    [Fact]
    public async Task WhenGetBooks_BooksReturned()
    {
        //Arrange
        var fixture = new Fixture();
        var books = fixture.CreateMany<Book>(3).ToList();
        await testDbContext.BulkInsertAsync(books);
        Assert.Equal(3, testDbContext.Books.Count());
        
        //Act
        var res = await sut.GetBooksAsync();
        
        //Assert

        var enumerable = res.ToList();
        Assert.Equal(3,enumerable.Count);
        enumerable.Should().BeEquivalentTo(books, options=>options.Excluding(o=>o.Id));

    }
    
    [Fact]
    public async Task WhenNoBooks_EmptyCollectionReturned()
    {
       
        //Act
        var res = await sut.GetBooksAsync();
        
        //Assert
        res.Should().BeEmpty();

    }
    
}