using AutoFixture;
using FirstApi.DTO;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FirstApi.test.Controllers.Tests.BooksController.Tests;

public class CreateBookTests
{
    [Fact]
    public async Task WhenCreateBook_ReturnsId()
    {
        var fixture = new Fixture();
        var newBook = fixture.Create<BookDto>();
        var bookServiceMock = new Mock<IBooksService>();
        bookServiceMock.Setup(bs => bs.CreateBookAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<BookTypes?>(),
                It.IsAny<int?>()))
            .ReturnsAsync("ok");
        var sut = new FirstApi.Controllers.BooksController(bookServiceMock.Object);
        var res = await sut.CreateBookAsync(newBook);

        res.Result.Should().BeOfType(typeof(OkObjectResult));
        var result = (res.Result as OkObjectResult);
        result.Should().NotBe(null);
        result.Value.Equals("ok").Should().BeTrue();
    }
    
    [Fact]
    public async Task WhenBookTypeValid_TypeSentToService()
    {
        var newBook = new BookDto("test", "test", 1, 2000);
        var bookServiceMock = new Mock<IBooksService>();
        bookServiceMock.Setup(bs => bs.CreateBookAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<BookTypes?>(),
                It.IsAny<int?>()))
            .ReturnsAsync("ok");
        var sut = new FirstApi.Controllers.BooksController(bookServiceMock.Object);
        await sut.CreateBookAsync(newBook);
        
        bookServiceMock.Verify(_ => _.CreateBookAsync(
            It.Is<string>(s => s == newBook.Title),
            It.Is<string>(s => s == newBook.Author),
            It.Is<BookTypes>(i=> i==(BookTypes)newBook.Type!), It.Is<int>(i => i == newBook.PublicationYear)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(8)]
    public async Task WhenBookTypeInvalid_SendNullToService(int? bookType)
    {
        var newBook = new BookDto("test", "test", bookType, 2000);
        var bookServiceMock = new Mock<IBooksService>();
        bookServiceMock.Setup(bs => bs.CreateBookAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<BookTypes?>(),
                It.IsAny<int?>()))
            .ReturnsAsync("ok");
        var sut = new FirstApi.Controllers.BooksController(bookServiceMock.Object);
        var res = await sut.CreateBookAsync(newBook);

        bookServiceMock.Verify(_ => _.CreateBookAsync(
            It.Is<string>(s => s == newBook.Title),
            It.Is<string>(s => s == newBook.Author),
            null, It.Is<int>(i => i == newBook.PublicationYear)));
    }
}