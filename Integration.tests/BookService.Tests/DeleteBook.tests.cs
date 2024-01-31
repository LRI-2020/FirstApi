using AutoFixture;
using FirstApi.Services;
using FluentAssertions;

namespace Integration.tests.BookService.Tests;

public class DeleteBookTests
{
    private readonly ApplicationDbContext testDbContext;
    private readonly BooksService sut;
    private readonly Fixture fixture;

    public DeleteBookTests()
    {
        fixture = new Fixture();
        testDbContext = TestHelper.GetConfiguredTestDbContext();
        sut = new BooksService(testDbContext);
        TestHelper.CleanDb(testDbContext);
    }

   [Fact]
    public async Task WhenDeleteById_BookDeleted()
    {
        //Arrange
        await TestHelper.CreateBooksInDbAsync(3, fixture, testDbContext);
        var ids = testDbContext.Books.Select(b => b.Id).ToList();
        var idToDelete = ids[new Random().Next(0,ids.Count)];
        (await testDbContext.Books.FindAsync(idToDelete)).Should().NotBeNull();
        
        //Act

        var r = await sut.DeleteByIdAsync(idToDelete);
        r.Should().BeTrue();
        var books = testDbContext.Books;
        books.Count().Should().Be(ids.Count- 1);
        (await testDbContext.Books.FindAsync(idToDelete)).Should().BeNull();

    }   
    
 [Fact]
    public async Task WhenDeleteNonExistingId_()
    {
        //Arrange
        await TestHelper.CreateBooksInDbAsync(3, fixture, testDbContext);
        var ids = testDbContext.Books.Select(b => b.Id).ToList();
        var idToDelete = new Random().Next(ids.Max(),Int32.MaxValue);
        (await testDbContext.Books.FindAsync(idToDelete)).Should().BeNull();
        
        //Act

        Func<Task> act = () => sut.DeleteByIdAsync(idToDelete);
        await act.Should().ThrowAsync<Exception>();
        
        var books = testDbContext.Books;
        books.Count().Should().Be(ids.Count);
    }   
    
    [Fact]
    public async Task WhenDeleteAll_AllBooksDeleted()
    {
        //Arrange
        var count = new Random().Next(2, 12);
        await TestHelper.CreateBooksInDbAsync(count, fixture, testDbContext);
        
        //Act
        var r = await sut.DeleteAllAsync();
        
        //Assert

        r.Should().Be(count);
        var books = testDbContext.Books.ToList();
        books.Should().BeEmpty();
    }    
    
    [Fact]
    public async Task WhenDeleteAllInEmptyDB_ZeroReturned()
    {
        //Arrange
        testDbContext.Books.Count().Should().Be(0);
        
        //Act
        var r = await sut.DeleteAllAsync();
        
        //Assert
        r.Should().Be(0);

    }

}