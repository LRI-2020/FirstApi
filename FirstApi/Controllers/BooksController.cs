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
        if (book==null) 
            return NotFound(new ProblemDetails(){Title = $"Book with Id {id} not found"});
        return Ok(book);
    }

    [HttpPost]
    public ActionResult<string> Post(BookRequestDto book)
    {
        
            return Ok(booksService.CreateBook(book.Title, book.Author, book.Type,book.Year).Result);
        }

    [HttpDelete("all")] //Will listen to /books/all
    public ActionResult<int> DeleteAll()
    {
        throw new NotImplementedException();
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

        var updatedBookDto = new BookRequestDto(book.Title, book.Author, book.Type, book.PublicationYear);
        bookUpdates.ApplyTo(updatedBookDto);

        book.Title = updatedBookDto.Title;
        book.Author = updatedBookDto.Author;
        book.Type = updatedBookDto.Type;
        book.PublicationYear = updatedBookDto.Year;

        return Ok(booksService.UpdateBook(id, book).Result);

    }
}