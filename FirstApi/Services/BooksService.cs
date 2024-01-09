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
        using HttpResponseMessage res = await httpClient.GetAsync(ApiUrl + ".json");
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
                {
                    var currentBookType = Enum.IsDefined(typeof(BookTypes), rawBook.Type);
                    books.Add(new Book(rawBook.Title, rawBook.Author,
                        currentBookType ? Enum.Parse<BookTypes>(rawBook.Type, true) : BookTypes.Unknown,
                        rawBook.PublicationYear, name));
                }
            }
        }

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

    public async Task<string> CreateBook(string title, string author, BookTypes type, int publicationYear)
    {
        var rawBook = new RawBook(title, author, publicationYear, type.ToString());
        var body = JsonSerializer.Serialize(rawBook);
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        using HttpResponseMessage res = await httpClient.PostAsync(ApiUrl + ".json", requestContent);
        if (!res.IsSuccessStatusCode) throw new Exception("Book not created");

        return GetJsonBodyResponse(res).Result["name"]!.ToString();
    }

    public async Task<Book> UpdateBook(string id, Book book)
    {
        if (id != book.Id)
            throw new Exception("Id does not match with the book Id");
        var rawBook = new RawBook(book.Title, book.Author, book.PublicationYear, book.Type.ToString());

        var body = JsonSerializer.Serialize(rawBook);
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        using HttpResponseMessage res = await httpClient.PatchAsync(ApiUrl + "/" + book.Id + ".json", requestContent);
        if (!res.IsSuccessStatusCode) throw new Exception("book could not be patched");

        var result = JObject.Parse(await res.Content.ReadAsStringAsync());
        var patchedBook = result.ToObject<RawBook>();

        if (patchedBook == null || !BookConverter.IsBook(patchedBook))
            throw new Exception("book could not be patched");
        return new Book(patchedBook.Title, patchedBook.Author, (BookTypes)Enum.Parse(typeof(BookTypes), patchedBook.Type, true), patchedBook.PublicationYear, id);
    }

    private async Task<JObject> GetJsonBodyResponse(HttpResponseMessage res)
    {
        var jsonResponse = await res.Content.ReadAsStringAsync();
        JObject result = JObject.Parse(jsonResponse);
        return result;
    }

    public async Task<int> DeleteAll()
    {
        var booksCount = GetBooks().Result.Count();
        using HttpResponseMessage res = await httpClient.DeleteAsync(ApiUrl + ".json");
        if (res.IsSuccessStatusCode)
            return booksCount;

        throw new Exception("Error occurred when deleting the books - " + res.StatusCode);
    }

    public async Task<bool> DeleteById(string id)
    {
        if (await GetBookById(id) == null)
            throw new KeyNotFoundException($"Book with Id {id} does not exist");

        using HttpResponseMessage res = await httpClient.DeleteAsync(ApiUrl + $"/{id}.json");
        if (res.IsSuccessStatusCode)
            return true;
        return false;
    }
}