using System.ComponentModel.DataAnnotations;
using FirstApi.Models;
using FirstApi.ValidationAttributes;

namespace FirstApi.DTO;

public class BookRequestDto
{
    [Required]
    [MinLength(2)]
    public string Title { get; set; }
    [Required]
    [MinLength(2)]
    public string Author { get; set; }
    public int Type { get; set; }
    [MinValue(1700)]
    public int Year { get; set; }
    public BookRequestDto(string title, string author, int? type, int? year)
    {
        Title = title;
        Author = author;
        Type = type??0;
        Year = year ?? 1970;
    }
}