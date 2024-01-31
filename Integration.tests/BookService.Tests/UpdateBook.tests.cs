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

        var updatedBook = SetBookNewValues(originalBook,newTitle,newAuthor,newPubYear);
        
        //Act
       await sut.UpdateBookAsync(originalBook.Id, updatedBook);
       var r = await testDbContext.Books.FindAsync(originalBook.Id); 
       
        //Assert
        r.Should().BeEquivalentTo(updatedBook);
    }

    private Book SetBookNewValues(Book originalBook, string newTitle, string newAuthor, int newPubYear)
    {
        var newRatings = fixture.CreateMany<int>(4).ToList();
        var newType = BookTypes.Comic;

        originalBook.Title = newTitle;
        originalBook.Author = newAuthor;
        originalBook.PublicationYear = newPubYear;
        originalBook.Ratings = newRatings;
        originalBook.Type = newType;

        return originalBook;
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