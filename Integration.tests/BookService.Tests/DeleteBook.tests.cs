using AutoFixture;
using AutoFixture.Xunit2;
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
    
    [Theory]
    [AutoData]
    public void WhenDeleteNonExistingId_()
    {
        
    }   
    
    [Theory]
    [AutoData]
    public void WhenDeleteAll_AllBooksDeleted()
    {
        
    }    
    
    [Theory]
    [AutoData]
    public void WhenDeleteAllInEmptyDB_ZeroReturned()
    {
        
    }

}