using System.Text;
using System.Text.Json;
using FirstApi.Models;
using Newtonsoft.Json.Linq;

namespace FirstApi.Services;

public class BooksService : IBooksService
{
    private readonly HttpClient httpClient;

    public BooksService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    private string ApiUrl { get; set; } = "https://first-api-eeffd-default-rtdb.europe-west1.firebasedatabase.app/books";

    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        var books = new List<Book>();

        using HttpResponseMessage res = await httpClient.GetAsync(ApiUrl+".json");
        var jsonResponse = await res.Content.ReadAsStringAsync();
        if (jsonResponse == "null") return books;
        JObject rawBooks = JObject.Parse(jsonResponse);

        foreach (var x in rawBooks)
        {
            string name = x.Key;
            JToken? value = x.Value;
            if (value != null)
            {
                var rawBook = value.ToObject<FireBaseBook>();
                if (rawBook != null)
                {
                    var currentBookType = Enum.IsDefined(typeof(BookTypes), rawBook.Type);
                    books.Add(new Book(rawBook.Title, rawBook.Author, 
                        currentBookType ? Enum.Parse<BookTypes>(rawBook.Type, true): BookTypes.Unknown, 
                        rawBook.PublicationYear, name));
                }
            }
        }

        //throw new Exception("error occurred server side");
         return books;
    }

    public async Task<Book?> GetBookByIdAsync(string id)
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
        var rawBook = new FireBaseBook(title, author, publicationYear??1970, type.ToString()??BookTypes.Unknown.ToString());
        var body = JsonSerializer.Serialize(rawBook);
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        using HttpResponseMessage res = await httpClient.PostAsync(ApiUrl+".json", requestContent);
        if (!res.IsSuccessStatusCode) throw new Exception("Book not created");

        return (await GetJsonBodyResponseAsync(res))["name"]!.ToString();
    }

    public async Task<Book> UpdateBookAsync(string id, Book book)
    {
        if (id != book.Id)
            throw new Exception("Id does not match with the book Id");
        
        var fireBaseBook = new FireBaseBook(book.Title, book.Author, book.PublicationYear, book.Type.ToString());
        var body = JsonSerializer.Serialize(fireBaseBook);
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        using HttpResponseMessage res = await httpClient.PutAsync(ApiUrl + "/" + book.Id+".json", requestContent);
        if (!res.IsSuccessStatusCode) throw new Exception("book could not be updated"); 

        var result = JObject.Parse(await res.Content.ReadAsStringAsync());
        var updatedFirebaseBook = result.ToObject<FireBaseBook>();
        
        if(updatedFirebaseBook==null || !BookConverter.IsBook(updatedFirebaseBook))
            throw new Exception("book could not be updated");
        return new Book(updatedFirebaseBook.Title, updatedFirebaseBook.Author, (BookTypes)Enum.Parse(typeof(BookTypes),updatedFirebaseBook.Type, true), updatedFirebaseBook.PublicationYear, id);
    }

    private async Task<JObject> GetJsonBodyResponseAsync(HttpResponseMessage res)
    {
        var jsonResponse = await res.Content.ReadAsStringAsync();
        JObject result = JObject.Parse(jsonResponse);
        return result;
    }
    
    public async Task<int> DeleteAllAsync()
    {
        var booksCount = (await GetBooksAsync()).Count();
        using var res = await httpClient.DeleteAsync(ApiUrl + ".json");
        if (res.IsSuccessStatusCode)
            return booksCount;

        throw new Exception("Error occurred when deleting the books - " + res.StatusCode);
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        if (await GetBookByIdAsync(id) == null)
            throw new KeyNotFoundException($"Book with Id {id} does not exist");

        using HttpResponseMessage res = await httpClient.DeleteAsync(ApiUrl + $"/{id}.json");
        return res.IsSuccessStatusCode;
    }
    
}