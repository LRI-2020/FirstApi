using AutoFixture;
using FirstApi.Models;
using FirstApi.Services;
using Moq;

namespace FirstApi.test.Controllers.Tests.BooksController.Tests;

public static class SharedSetup
{
    public static Mock<IBooksService> BasicBookServiceMockSetup(Mock<IBooksService> bookServiceMock)
    {
        var fixture = new Fixture();
        var book = fixture.Create<Book>();
        bookServiceMock.Setup(bs =>
                bs.GetBookByIdAsync(
                    It.IsAny<int>()))
            .ReturnsAsync(book);        
        
        bookServiceMock.Setup(bs =>
                bs.UpdateBookAsync(
                    It.IsAny<int>(), It.IsAny<Book>()))
            .ReturnsAsync(book);

        return bookServiceMock;
    }
}