using System.ComponentModel.DataAnnotations;

namespace FirstApi.DTO;

public class BookRequestDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Author { get; set; }
    public string Type { get; set; }
    public int Year { get; set; }
    public BookRequestDto(string title, string author, string? type, int? year)
    {
        Title = title;
        Author = author;
        Type = type ?? "Unknown";
        Year = year ?? 1970;
    }
}