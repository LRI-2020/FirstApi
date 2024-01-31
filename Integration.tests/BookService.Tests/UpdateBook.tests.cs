using AutoFixture;
using AutoFixture.Xunit2;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;

namespace Integration.tests.BookService.Tests;



public class UpdateBookTests
{
    private readonly ApplicationDbContext testDbContext;
    private readonly BooksService sut;
    private readonly Fixture fixture;

    public UpdateBookTests()
    {
        fixture = new Fixture();
        testDbContext = TestHelper.GetConfiguredTestDbContext();
        sut = new BooksService(testDbContext);
        TestHelper.CleanDb(testDbContext);
    }

    [Theory]
    [AutoData]
    public async Task WhenUpdate_BookUpdated(string newTitle, string newAuthor, int newPubYear)
    {
        //Arrange
        var originalBook = fixture.Build<Book>().Without(b => b.Id).Create();
        originalBook.Type = BookTypes.Biography;
        testDbContext.Books.Add(originalBook);
        await testDbContext.SaveChangesAsync();
        
        var newRatings = fixture.CreateMany<int>(4).ToList();
        var newType = BookTypes.Comic;
        var updatedBook = originalBook;
        updatedBook.Type = newType;
        updatedBook.PublicationYear = newPubYear;
        updatedBook.Title = newTitle;
        updatedBook.Author = newAuthor;
        updatedBook.Ratings = newRatings;
        
        //Act

        var r =await sut.UpdateBookAsync(originalBook.Id, updatedBook);
        //Assert
        r.Id.Should().Be(originalBook.Id);
        r.Type.Should().Be(newType);
        r.PublicationYear.Should().Be(newPubYear);
        r.Title.Should().Be(newTitle);
        r.Author.Should().Be(newAuthor);
        r.Ratings.Should().BeEquivalentTo(newRatings);

    }    
    
    [Fact]
    public async Task WhenUpdate_BookIdNotUpdated()
    {
        //Arrange
        var originalBook = fixture.Build<Book>().Without(b => b.Id).Create();
        originalBook.Type = BookTypes.Biography;
        testDbContext.Books.Add(originalBook);
        await testDbContext.SaveChangesAsync();

        var updatedBook = fixture.Build<Book>()
            .With(b => b.Id, new Random().Next(originalBook.Id + 1, Int32.MaxValue))
            .Create();
                
        Func<Task> act = () => sut.UpdateBookAsync(originalBook.Id,updatedBook);
        await act.Should().ThrowAsync<Exception>();
    }


}