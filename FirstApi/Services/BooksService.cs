using System.Reflection;
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

    private string ApiUrl
    {
        get;
        set;
    } = "https://first-api-eeffd-default-rtdb.europe-west1.firebasedatabase.app/books.json";

    public async Task<IEnumerable<Book>> GetBooks()
    {
        using HttpResponseMessage res = await httpClient.GetAsync(ApiUrl);
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
                        books.Add(new Book(rawBook.Title, rawBook.Author, rawBook.Type, rawBook.Publication_year, name));
                }
            }
            return books;
    }
}