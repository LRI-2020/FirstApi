using FirstApi.DTO;

namespace FirstApi.Models;

public class Book
{
    public Book(string title, string author, BookTypes? type, int? publicationYear, string id)
    {
        Title = title;
        Author = author;
        Type = type?? BookTypes.Unknown;
        PublicationYear = publicationYear??1970;
        Id = id;
    }

    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public BookTypes Type { get; set; }
    public string Id { get; set; }
}

public static class BookConverter
{
    public static bool IsBook(FireBaseBook fireBaseBook)
    {
        return Enum.IsDefined(typeof(BookTypes), fireBaseBook.Type);
    }

}

public class FireBaseBook
{
    public FireBaseBook(string title, string author, int publicationYear, string type)
    {
        Title = title;
        Author = author;
        PublicationYear = publicationYear;
        Type = type;
    }

    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public string Type { get; set; }
}