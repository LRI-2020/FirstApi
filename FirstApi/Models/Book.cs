namespace FirstApi.Models;

public class Book
{
    public Book(string title, string author, string type, int publicationYear, string id)
    {
        Title = title;
        Author = author;
        Type = type;
        PublicationYear = publicationYear;
        Id = id;
    }

    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public string Type { get; set; }
    public string Id { get; set; }
}



public class RawBook
{
    public RawBook(string title, string author, int publicationYear, string type)
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