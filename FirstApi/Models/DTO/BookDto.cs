using System.ComponentModel.DataAnnotations;
using FirstApi.Models;
using FirstApi.ValidationAttributes;
using FluentValidation;

namespace FirstApi.DTO;

public class BookDto
{
    [Required] [MinLength(2)] public string Title { get; set; }
    [Required] [MinLength(2)] public string Author { get; set; }
    public List<int> Ratings { get; set; } = new();
    public int? Type { get; set; }
    [MinValue(1700)] public int? PublicationYear { get; set; }

    public BookDto(string title, string author, int? type, int? year)
    {
        Title = title;
        Author = author;
        Type = type;
        PublicationYear = year;
    }
}
