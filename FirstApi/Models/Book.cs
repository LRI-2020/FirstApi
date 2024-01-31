using FirstApi.DTO;

namespace FirstApi.Models;
using System.ComponentModel.DataAnnotations;

public class Book
{

    public Book()
    {
        
    }
   
    public Book(string title, string author, BookTypes? type, int? publicationYear, List<int>?ratings)
    {
        Title = title;
        Author = author;
        Type = type?? BookTypes.Unknown;
        PublicationYear = publicationYear??1970;
        Ratings = ratings ?? new List<int>();
    }
    
    [Key]
    public int Id { get; set; }

    public List<int> Ratings { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public BookTypes Type { get; set; }
    
}