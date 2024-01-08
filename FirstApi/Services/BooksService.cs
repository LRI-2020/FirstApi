using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json;
using FirstApi.Models;
using Newtonsoft.Json.Linq;

namespace FirstApi.Services;

public class BooksService
{
    private readonly HttpClient httpClient;

    public BooksService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    private string ApiUrl { get; set; } = "https://first-api-eeffd-default-rtdb.europe-west1.firebasedatabase.app/books";

    public async Task<IEnumerable<Book>> GetBooks()
    {
        using HttpResponseMessage res = await httpClient.GetAsync(ApiUrl+".json");
        var jsonResponse = await res.Content.ReadAsStringAsync();
        JObject rawBooks = JObject.Parse(jsonResponse);

        var books = new List<Book>();
        foreach (var x in rawBooks)
        {
            string name = x.Key;
            JToken? value = x.Value;
            if (value != null)
            {
                var rawBook = value.ToObject<RawBook>();
                if (rawBook != null)
                    books.Add(new Book(rawBook.Title, rawBook.Author, rawBook.Type, rawBook.PublicationYear, name));
            }
        }

        //throw new Exception("error occurred server side");
         return books;
    }

    public async Task<Book?> GetBookById(string id)
    {
        var books = await GetBooks();

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

    public async Task<string> CreateBook(string title, string author, string type, int publicationYear)
    {
        var rawBook = new RawBook(title, author, publicationYear, type);
        var body = JsonSerializer.Serialize(rawBook);
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        using HttpResponseMessage res = await httpClient.PostAsync(ApiUrl+".json", requestContent);
        if (!res.IsSuccessStatusCode) throw new Exception("Book not created");

        return GetJsonBodyResponse(res).Result["name"]!.ToString();
    }

    public async Task<Book> UpdateBook(string id, Book book)
    {
        if (id != book.Id)
            throw new Exception("Id does not match with the book Id");
        var rawBook = new RawBook(book.Title, book.Author, book.PublicationYear, book.Type);
        
        var body = JsonSerializer.Serialize(rawBook);
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        using HttpResponseMessage res = await httpClient.PatchAsync(ApiUrl + "/" + book.Id+".json", requestContent);
        if (!res.IsSuccessStatusCode) throw new Exception("book could not be patched"); 

        var result = JObject.Parse(await res.Content.ReadAsStringAsync());
        var patchedBook = result.ToObject<RawBook>();
        
        if(patchedBook==null)
            throw new Exception("book could not be patched");
        return new Book(patchedBook.Title, patchedBook.Author, patchedBook.Type, patchedBook.PublicationYear, id);


    }

    private async Task<JObject> GetJsonBodyResponse(HttpResponseMessage res)
    {
        var jsonResponse = await res.Content.ReadAsStringAsync();
        JObject result = JObject.Parse(jsonResponse);
        return result;
    }
}