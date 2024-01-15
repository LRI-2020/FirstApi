using FirstApi.Models;

namespace FirstApi.Services;

public interface IBooksService
{
    public Task<IEnumerable<Book>> GetBooksAsync();
    public Task<Book?> GetBookByIdAsync(string id);
    public  Task<string> CreateBookAsync(string title, string author, BookTypes? type, int? publicationYear);
    public  Task<Book> UpdateBookAsync(string id, Book book);
    public Task<int> DeleteAllAsync();
    public Task<bool> DeleteByIdAsync(string id);

}