using System.ComponentModel.Design;
using System.Text;
using System.Text.Json;
using EFCore.BulkExtensions;
using FirstApi.Models;
using Newtonsoft.Json.Linq;

namespace FirstApi.Services;

public class BooksService : IBooksService
{
    private readonly ApplicationDbContext applicationDbContext;

    public BooksService(ApplicationDbContext applicationDbContext)
    {
        this.applicationDbContext = applicationDbContext;
    }

    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        var books = applicationDbContext.Books.ToList();
        return books;
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        var books = await GetBooksAsync();

        try
        {
            return books.First(b => b.Id == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<int> CreateBookAsync(string title, string author, BookTypes? type, int? publicationYear, List<int>?ratings)
    {
        var newBook = new Book(title, author, type, publicationYear, ratings);
        applicationDbContext.Books.Add(newBook);
        await applicationDbContext.SaveChangesAsync();
        return newBook.Id;
    }

    public async Task<Book> UpdateBookAsync(int id, Book book)
    {
        if (id != book.Id)
            throw new Exception("Id does not match with the book Id");

        applicationDbContext.Books.Update(book);
        await applicationDbContext.SaveChangesAsync();
        return book;
    }


    public async Task<int> DeleteAllAsync()
    {
        var countBefore = applicationDbContext.Books.Count();
        await applicationDbContext.BulkDeleteAsync(applicationDbContext.Books);
        var countAfter = applicationDbContext.Books.Count();
        if (countAfter != 0)
            throw new Exception($"An error occurred - {countAfter} books could not be deleted");
        return countBefore;

    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var book = await applicationDbContext.Books.FindAsync(id);
        if (book == null)
            throw new KeyNotFoundException($"Book with Id {id} does not exist");

        applicationDbContext.Books.Remove(book);
        var r = await applicationDbContext.SaveChangesAsync();
        return await applicationDbContext.Books.FindAsync(id) == null;
    }
}