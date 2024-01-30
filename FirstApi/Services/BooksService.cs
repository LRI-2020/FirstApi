using System.Text;
using System.Text.Json;
using FirstApi.Models;
using Newtonsoft.Json.Linq;

namespace FirstApi.Services;

public class BooksService : IBooksService
{
    private readonly HttpClient httpClient;
    private readonly ApplicationDbContext applicationDbContext;

    public BooksService(HttpClient httpClient, ApplicationDbContext applicationDbContext)
    {
        this.httpClient = httpClient;
        this.applicationDbContext = applicationDbContext;
    }

    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        var books = new List<Book>();
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

    public async Task<string> CreateBookAsync(string title, string author, BookTypes? type, int? publicationYear)
    {
        var t = new Task<string>(() => "ok");
        t.Start();
        return await t;
    }

    public async Task<Book> UpdateBookAsync(int id, Book book)
    {
        
        var t = new Task<Book>(() => new Book("test","test",BookTypes.Biography,2000,1));
        t.Start();
        return await t;        
        //if (id != book.Id)
    //         throw new Exception("Id does not match with the book Id");
    //     
    //     var fireBaseBook = new FireBaseBook(book.Title, book.Author, book.PublicationYear, book.Type.ToString());
    //     var body = JsonSerializer.Serialize(fireBaseBook);
    //     var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
    //     using HttpResponseMessage res = await httpClient.PutAsync(ApiUrl + "/" + book.Id+".json", requestContent);
    //     if (!res.IsSuccessStatusCode) throw new Exception("book could not be updated"); 
    //
    //     var result = JObject.Parse(await res.Content.ReadAsStringAsync());
    //     var updatedFirebaseBook = result.ToObject<FireBaseBook>();
    //     
    //     if(updatedFirebaseBook==null || !BookConverter.IsBook(updatedFirebaseBook))
    //         throw new Exception("book could not be updated");
    //     return new Book(updatedFirebaseBook.Title, updatedFirebaseBook.Author, (BookTypes)Enum.Parse(typeof(BookTypes),updatedFirebaseBook.Type, true), updatedFirebaseBook.PublicationYear, id);
    }

    
    public async Task<int> DeleteAllAsync()
    {
        var t = new Task<int>(() => 1);
        t.Start();
        return await t;
        // var booksCount = (await GetBooksAsync()).Count();
        // using var res = await httpClient.DeleteAsync(ApiUrl + ".json");
        // if (res.IsSuccessStatusCode)
        //     return booksCount;
        //
        // throw new Exception("Error occurred when deleting the books - " + res.StatusCode);
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var t = new Task<bool>(() => true);
        t.Start();
        return await t;
        // if (await GetBookByIdAsync(id) == null)
        //     throw new KeyNotFoundException($"Book with Id {id} does not exist");
        //
        // using HttpResponseMessage res = await httpClient.DeleteAsync(ApiUrl + $"/{id}.json");
        // return res.IsSuccessStatusCode;
    }
    
}