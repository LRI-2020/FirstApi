﻿using AutoFixture;
using EFCore.BulkExtensions;
using FirstApi.Models;
using FirstApi.Services;
using FluentAssertions;

namespace Integration.tests.BookService.Tests;

public class GetBooksTests
{
    private readonly ApplicationDbContext testDbContext;
    private readonly BooksService sut;
    private readonly Fixture fixture;

    public GetBooksTests()
    {
        fixture = new Fixture();
        testDbContext = TestHelper.GetConfiguredTestDbContext();
        sut = new BooksService(testDbContext);
        TestHelper.CleanDb(testDbContext);
    }

    [Fact]
    public async Task WhenGetBooks_BooksReturned()
    {
        //Arrange
        
        var books = await CreateBooksInDbAsync(3);
        
        //Act
        var res = await sut.GetBooksAsync();
        
        //Assert

        var enumerable = res.ToList();
        Assert.Equal(3,enumerable.Count);
        enumerable.Should().BeEquivalentTo(books, options=>options.Excluding(o=>o.Id));

    }

    private async Task<IEnumerable<Book>> CreateBooksInDbAsync(int count)
    {
        var booksData = fixture.CreateMany<Book>(count).ToList();
        await testDbContext.BulkInsertAsync(booksData);
        var res = testDbContext.Books;
        Assert.Equal(count, res.Count());
        return res;
    }

    [Fact]
    public async Task WhenNoBooks_EmptyCollectionReturned()
    {
       
        //Act
        var res = await sut.GetBooksAsync();
        
        //Assert
        res.Should().BeEmpty();

    }

    [Fact]

    public async Task WhenGetById_MatchingBookReturnedIfExists()
    {
        //Arrange
        var books = (await CreateBooksInDbAsync(3)).ToList();
        var id = books.Select(b => b.Id).FirstOrDefault();
        
        //Act
        var res = await sut.GetBookByIdAsync(id);
        
        //Assert
        res.Should().NotBeNull();
        res.Should().BeEquivalentTo(books.First(b => b.Id == id));

    }    
    
    [Fact]

    public async Task WhenGetById_NullReturnedIfBookNotExists()
    {
        //Arrange
        await CreateBooksInDbAsync(3);
        var lastExistingId = testDbContext.Books.Select(b => b.Id).Max();
        var nonExistingId = new Random().Next(lastExistingId,Int32.MaxValue);
        
        //Act
        var res = await sut.GetBookByIdAsync(nonExistingId);
        
        //Assert
        res.Should().BeNull();
    }
    
}