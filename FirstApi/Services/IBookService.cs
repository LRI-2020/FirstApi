using FirstApi.Models;

namespace FirstApi.Services;

public interface IBooksService
{
    public Task<IEnumerable<Book>> GetBooksAsync();
    public Task<Book?> GetBookByIdAsync(int id);
    public  Task<int> CreateBookAsync(string title, string author, BookTypes? type, int? publicationYear);
    public  Task<Book> UpdateBookAsync(int id, Book book);
    public Task<int> DeleteAllAsync();
    public Task<bool> DeleteByIdAsync(int id);

}