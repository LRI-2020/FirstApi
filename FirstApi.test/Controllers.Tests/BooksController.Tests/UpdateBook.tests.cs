using System.Net.Mail;
using AutoFixture;
using AutoFixture.Xunit2;
using Castle.Components.DictionaryAdapter;
using FirstApi.DTO;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        var res  = await sut.UpdateBookAsync("id", bookDto);
        
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
                  It.Is<string>(s => s == originalBook.Id)))
          .ReturnsAsync(originalBook);
      Book? bookSentToService = null;
      
      bookServiceMock.Setup(bs =>
              bs.UpdateBookAsync(
                  It.IsAny<string>(), It.IsAny<Book>()))
          .Callback<string, Book>((s, b) => bookSentToService = b)
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
                    It.IsAny<string>()))
            .ReturnsAsync((Book?)null);
        var res = await sut.UpdateBookAsync("id", bookDto);

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
                    It.IsAny<string>()))
            .ReturnsAsync(originalBook);     
        
        var paramPassed = new List<Book>();
        
        bookServiceMock.Setup(bs =>
            bs.UpdateBookAsync(It.IsAny<string>(),Capture.In(paramPassed)))
            .ReturnsAsync(originalBook);

        //Act
         await sut.UpdateBookAsync("id", bookDto);
        
         //Assert
         paramPassed[0].Type.Should().Be((originalBook).Type);
         paramPassed[0].PublicationYear.Should().Be((originalBook).PublicationYear);

    }
    
    [Fact]
    public void BasicStrings()
    {
        // arrange
        var fixture = new Fixture();
        // fixture.Customize(new CurrentDateTimeCustomization());
        fixture.Customizations.Add(new CurrentDateTimeGenerator());
        var currentDateTime = fixture.Create<DateTime>();
        var book = fixture.Build<Book>()
            .Without(b => b.PropNotInitializedThroughCtor)
            .With(b => b.Author, "J.K.Rowling")
            .Without(b => b.Ratings)
            .Do(b => b.Ratings.Add(5))
            .Create();


        // var string1 = fixture.Create<string>();
        // var string2 = fixture.Create<string>();
        //
        // testOutputHelper.WriteLine(string1);
        // testOutputHelper.WriteLine(string2);
         testOutputHelper.WriteLine(currentDateTime.ToString());
        // testOutputHelper.WriteLine(JsonSerializer.Serialize(book));
    }

}