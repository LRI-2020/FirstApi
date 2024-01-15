using AutoFixture;
using FirstApi.Controllers;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FirstApi.test.Controllers.Tests;

public class BooksControllerTests
{
    [Fact]
    public async Task WhenGetAll_ReturnsIEnumerableOfBooks()
    {
        var fixture = new Fixture();
        var books = fixture.CreateMany<Book>(3).ToArray();
        var bookServiceMock = new Mock<IBooksService>();
        bookServiceMock.Setup(bs => bs.GetBooksAsync()).ReturnsAsync(books);
        var sut = new BooksController(bookServiceMock.Object);
        
        var res = await sut.GetAll();
        
        res.Result.Should().BeOfType(typeof(OkObjectResult));
        var actualBooks = res.Result as OkObjectResult;
        actualBooks.Value.Should().BeEquivalentTo(books);
    }
}