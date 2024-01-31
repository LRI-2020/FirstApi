using AutoFixture;
using AutoFixture.Xunit2;
using FirstApi.DTO;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace FirstApi.test.Controllers.Tests.BooksController.Tests;

public class UpdateBookTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public UpdateBookTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenInputValid_UpdatedBookReturned(
        [Frozen]Mock<IBooksService> bookServiceMock,
        BookDto bookDto,
        [Greedy]FirstApi.Controllers.BooksController sut)
    {

        SharedSetup.BasicBookServiceMockSetup(bookServiceMock);

        var res  = await sut.UpdateBookAsync(1, bookDto);
        
        res.Result.Should().BeOfType(typeof(OkObjectResult));
        var ok = res.Result as OkObjectResult;
        ok!.Value.Should().BeOfType(typeof(Book));

    }

    [Theory]
    [AutoMoqData]
    public async Task WhenInputValid_NewValueSentToService(Fixture fixture, 
       [Frozen]Mock<IBooksService> bookServiceMock,
       [Greedy] FirstApi.Controllers.BooksController sut)
    {
      var originalBook = fixture.Build<Book>()
          .With(b => b.Type, BookTypes.Biography)
          .With(b => b.PublicationYear, 2000)
          .Create();
      
      var updatedBook = fixture.Build<Book>()
          .With(b => b.Type, BookTypes.Comic)
          .With(b => b.PublicationYear, 2020)
          .With(b => b.Id, originalBook.Id)
          .Create();
      
      var updatedBookDto = new BookDto(updatedBook.Title, updatedBook.Author, (int)updatedBook.Type, updatedBook.PublicationYear);
      
      
      bookServiceMock.Setup(bs =>
              bs.GetBookByIdAsync(
                  It.Is<int>(s => s == originalBook.Id)))
          .ReturnsAsync(originalBook);
      Book? bookSentToService = null;
      
      bookServiceMock.Setup(bs =>
              bs.UpdateBookAsync(
                  It.IsAny<int>(), It.IsAny<Book>()))
          .Callback<int, Book>((s, b) => bookSentToService = b)
          .ReturnsAsync(updatedBook);
         await sut.UpdateBookAsync(updatedBook.Id, updatedBookDto);
      
      bookSentToService.Should().NotBe(null);
      bookSentToService!.Title.Should().BeEquivalentTo(updatedBookDto.Title);
      bookSentToService.Author.Should().BeEquivalentTo(updatedBookDto.Author);
      ((int)bookSentToService.Type).Should().Be(updatedBookDto.Type);
      bookSentToService.PublicationYear.Should().Be(updatedBookDto.PublicationYear);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenBookNotFound_NotFoundReturned([Frozen] Mock<IBooksService> bookServiceMock, 
        [Greedy]FirstApi.Controllers.BooksController sut, 
        BookDto bookDto)
    {
        bookServiceMock.Setup(bs =>
                bs.GetBookByIdAsync(
                    It.IsAny<int>()))
            .ReturnsAsync((Book?)null);
        var res = await sut.UpdateBookAsync(1, bookDto);

        res.Result.Should().BeOfType(typeof(NotFoundResult));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenNullValues_PreviousValueKept(Fixture fixture,
        [Frozen] Mock<IBooksService> bookServiceMock,
        Book originalBook,
        [Greedy] FirstApi.Controllers.BooksController sut)
    {
        //Arrange
        
        var bookDto = fixture.Build<BookDto>()
            .With(b => b.Type,(int?)null)
            .With(b => b.PublicationYear,(int?)null)
            .Create();
        
        bookServiceMock.Setup(bs =>
                bs.GetBookByIdAsync(
                    It.IsAny<int>()))
            .ReturnsAsync(originalBook);     
        
        var paramPassed = new List<Book>();
        
        bookServiceMock.Setup(bs =>
            bs.UpdateBookAsync(It.IsAny<int>(),Capture.In(paramPassed)))
            .ReturnsAsync(originalBook);

        //Act
         await sut.UpdateBookAsync(1, bookDto);
        
         //Assert
         paramPassed[0].Type.Should().Be((originalBook).Type);
         paramPassed[0].PublicationYear.Should().Be((originalBook).PublicationYear);

    }

}