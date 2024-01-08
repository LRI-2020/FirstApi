using System.ComponentModel.DataAnnotations;
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
    public string Type { get; set; }
    [MinValue(1700)]
    public int Year { get; set; }
    public BookRequestDto(string title, string author, string? type, int? year)
    {
        Title = title;
        Author = author;
        Type = type ?? "Unknown";
        Year = year ?? 1970;
    }
}