using AutoFixture;
using FirstApi.Controllers;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FirstApi.test.Controllers.Tests.BooksController.Tests;

public class GetAllBooksTests
{
    [Fact]
    public async Task WhenGetAll_ReturnsIEnumerableOfBooks()
    {
        var fixture = new Fixture();
        var books = fixture.CreateMany<Book>(3).ToArray();
        var bookServiceMock = new Mock<IBooksService>();
        bookServiceMock.Setup(bs => bs.GetBooksAsync()).ReturnsAsync(books); 
        var sut = new FirstApi.Controllers.BooksController(bookServiceMock.Object);
        
        var res = await sut.GetAllBooks();
        
        res.Result.Should().BeOfType(typeof(OkObjectResult));
        var actualBooks = res.Result as OkObjectResult;
        actualBooks!.Value.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task WhenNoBooks_ReturnsNotFound()
    {
        var bookServiceMock = new Mock<IBooksService>();
        bookServiceMock.Setup(bs => bs.GetBooksAsync()).ReturnsAsync(new List<Book>());
        var sut = new FirstApi.Controllers.BooksController(bookServiceMock.Object);
        
        var res = await sut.GetAllBooks();
        
        res.Result.Should().BeOfType(typeof(NotFoundResult));
    }      
  
    
    [Fact]
    public async Task WhenErrorInService_Returns500()
    {
        var bookServiceMock = new Mock<IBooksService>();
        var exception = new Exception("error occurred");
        bookServiceMock.Setup(bs => bs.GetBooksAsync()).Throws(exception);
        var sut = new FirstApi.Controllers.BooksController(bookServiceMock.Object);
        await Assert.ThrowsAsync<Exception>(() => sut.GetAllBooks());

        var mock = new Mock<ILicense>();
        mock.Setup(x => x.LicenseKey).Returns(GetLicenseKey);
        Assert.Equal("a new key",mock.Object.LicenseKey);
        
        
    }

    private string GetLicenseKey()
    {
        return "a new key";
    }

    public interface ILicense
    {
        public string LicenseKey { get; set; }
        
    }
    
    
}