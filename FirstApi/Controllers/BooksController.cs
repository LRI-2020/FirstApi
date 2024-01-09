using FirstApi.DTO;
using FirstApi.Models;
using FirstApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers;

[Route("[controller]")] //controller is going to listening to naything that comes to /<name of the controller>
[ApiController]
public class BooksController : Controller
{
    private readonly BooksService booksService;

    public BooksController(BooksService booksService)
    {
        this.booksService = booksService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Book>> Get()
    {
        var books = booksService.GetBooks().Result;
        var enumerable = books as Book[] ?? books.ToArray();
        if (!enumerable.Any())
            return NotFound();
        return Ok(enumerable);
    }

    [Route("/book/{id}")]
    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetById(string id)
    {
        var book = booksService.GetBookById(id).Result;
        if (book == null)
            return NotFound(new ProblemDetails() { Title = $"Book with Id {id} not found" });
        return Ok(book);
    }

    [HttpPost]
    public ActionResult<string> Post(BookRequestDto bookInput)
    {
        var isValidType = Enum.IsDefined(typeof(DayOfWeek), bookInput.Type);
        return Ok(booksService.CreateBook(bookInput.Title, bookInput.Author, isValidType ? (BookTypes)bookInput.Type : BookTypes.Unknown, bookInput.PublicationYear).Result);
    }

    [HttpDelete("all")] //Will listen to /books/all
    public ActionResult<int> DeleteAll()
    {
        return Ok(booksService.DeleteAll().Result);
    }

    [HttpDelete("{Id}")] //Will listen to /books/{id}
    public ActionResult<bool> DeleteById()
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{id}")]
    public ActionResult PatchBook(string id, JsonPatchDocument<BookRequestDto> bookUpdates)
    {
        var book = booksService.GetBookById(id).Result;
        if (book == null)
            return NotFound();

        var updatedBookDto = new BookRequestDto(book.Title, book.Author, (int)book.Type, book.PublicationYear);
        bookUpdates.ApplyTo(updatedBookDto);
        if (!ModelState.IsValid || !TryValidateModel(updatedBookDto))
        {
            return BadRequest(ModelState);
        }
        var isEnumIntParsed = Enum.IsDefined(typeof(DayOfWeek), updatedBookDto.Type);
        book.Title = updatedBookDto.Title;
        book.Author = updatedBookDto.Author;
        book.Type = isEnumIntParsed ? (BookTypes)updatedBookDto.Type : BookTypes.Unknown;
        book.PublicationYear = updatedBookDto.PublicationYear;
        return Ok(booksService.UpdateBook(id, book).Result);
    }
    
}