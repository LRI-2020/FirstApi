using AutoFixture;
using AutoFixture.Xunit2;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;

namespace Integration.tests.BookService.Tests;

public class CreateBookTests : IDisposable
{
    private readonly ApplicationDbContext testDbContext;
    private readonly BooksService sut;
    private readonly Fixture fixture;

    public CreateBookTests()
    {
        fixture = new Fixture();
        testDbContext = TestHelper.GetConfiguredTestDbContext();
        sut = new BooksService(testDbContext);
        TestHelper.CleanDb(testDbContext);

    }


    [Theory]
    [AutoData]
    public async Task WhenCreateBook_BookSavedInDb(string title, string author, BookTypes type, int publicationYear)
    {
        List<int> ratings = fixture.CreateMany<int>(new Random().Next(1,100)).ToList();
       var r =  await sut.CreateBookAsync(title, author, type, publicationYear,ratings);
        //Assert
        var newBook = testDbContext.Books.FindAsync(r);
        newBook.Should().NotBeNull();
    }    
    
    [Theory]
    [AutoData]
    public async Task WhenCreateBookWithNullableProp_BookSavedWithDefaultValue(string title, string author)
    {
       var r =  await sut.CreateBookAsync(title, author, null, null, null);
        //Assert
        var newBook = await testDbContext.Books.FindAsync(r);
        newBook.Should().NotBeNull();
        newBook!.Type.Should().Be(BookTypes.Unknown);
        newBook.PublicationYear.Should().Be(1970);
        newBook.Ratings.Should().BeEmpty();
    }

    public void Dispose()
    {
        testDbContext.Dispose();
    }
}