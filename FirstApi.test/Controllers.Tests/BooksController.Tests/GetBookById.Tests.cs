using AutoFixture;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FirstApi.test.Controllers.Tests.BooksController.Tests;

public class GetBookByIdTests
{
    [Fact]
    public async Task WhenExistingId_BookIsReturned()
    {
        var bookServiceMock = new Mock<IBooksService>();
        var fixture = new Fixture();
        var books = fixture.CreateMany<Book>(5).ToArray();
        var ids = new List<string>();
        books.ForEach(b => ids.Add(b.Id));
        var rand = new Random();
        var id = ids[rand.Next(5)];
        var index = ids.FindIndex(i => i == id);
        bookServiceMock.Setup(bs => bs.GetBookByIdAsync(id)).ReturnsAsync(books.First(b => b.Id == id));
        var sut = new FirstApi.Controllers.BooksController(bookServiceMock.Object);
       
        var res = await sut.GetBookByIdAsync(id);
        
        res.Result.Should().BeOfType(typeof(OkObjectResult));
        var actualBook = res.Result as OkObjectResult;
        actualBook.Value.Should().BeEquivalentTo(books[index]);
    }

    [Fact]
    public async Task WhenNonExistingId_NotFoundReturned()
    {
        var bookServiceMock = new Mock<IBooksService>();
        bookServiceMock.Setup(bs => bs.GetBookByIdAsync(It.IsAny<string>())).ReturnsAsync((Book?) null);
        var sut = new FirstApi.Controllers.BooksController(bookServiceMock.Object);
       
        var res = await sut.GetBookByIdAsync("abc");
        
        res.Result.Should().BeOfType(typeof(NotFoundObjectResult));
    }

    
}