using FirstApi.DTO;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetTopologySuite.Utilities;
using Xunit;
namespace FirstApi.IntesgrationTests.BookService.Tests;

public class CreateBook_tests
{
    [Theory]
    public async Task WhenCreateBook_BookSavedInDb( ApplicationDbContext testDbContext,BooksService sut, 
        string title, string author, BookTypes type, int publicationYear)
    {
       var r =  await sut.CreateBookAsync(title, author, type, publicationYear);
        
        //Assert
        var newBook = testDbContext.Books.FindAsync(r);
        newBook.Should().NotBeNull();
    }
    
    

    
}