using FirstApi.DTO;
using FirstApi.Models;
using FirstApi.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers;

[Route("/books")]
[ApiController]
public class BooksController : Controller
{
    private readonly BooksService booksService;
    private readonly IValidator<BookDto> validator;

    public BooksController(BooksService booksService,   IValidator<BookDto> validator)
    {
        this.booksService = booksService;
        this.validator = validator;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAll()
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

    [Route("/book")]
    [HttpPost]
    public ActionResult<string> CreateBook(BookDto bookInput)
    {
        if (bookInput.Type == null) return Ok(booksService.CreateBook(bookInput.Title, bookInput.Author, null, bookInput.PublicationYear).Result);
        var isValidType = bookInput.Type != null && Enum.IsDefined(typeof(BookTypes), bookInput.Type);
        return Ok(booksService.CreateBook(bookInput.Title, bookInput.Author, isValidType ? (BookTypes)bookInput.Type! : null, bookInput.PublicationYear).Result);
    }

    [Route("/book/{id}")]
    [HttpPut]
    public ActionResult<string> UpdateBook(string id, BookDto bookInput)
    {
        var originalBook = booksService.GetBookById(id).Result;
        if (originalBook == null)
            return NotFound();
        //If nullable prop not passed, kept existing value
        var newType = bookInput.Type == null || !Enum.IsDefined(typeof(BookTypes), bookInput.Type) ? originalBook.Type : (BookTypes)bookInput.Type;
        var newDate = bookInput.PublicationYear ?? originalBook.PublicationYear;
        var updatedBook = new Book(bookInput.Title, bookInput.Author, newType, newDate, id);
        return Ok(booksService.UpdateBook(id, updatedBook).Result);
    }

    [HttpDelete("all")] //Will listen to /books/all
    public ActionResult<int> DeleteAll()
    {
        return Ok(booksService.DeleteAll().Result);
    }

    [Route("/book/{id}")]
    [HttpDelete] //Will listen to /books/{id}
    public ActionResult<bool> DeleteById(string id)
    {
        return Ok(booksService.DeleteById(id).Result);
    }

    [Route("/book/{id}")]
    [HttpPatch]
    public ActionResult PatchBook(string id, JsonPatchDocument<BookDto> bookUpdates)
    {
        var book = booksService.GetBookById(id).Result;
        if (book == null)
            return NotFound();

        var updatedBookDto = new BookDto(book.Title, book.Author, (int)book.Type, book.PublicationYear);
        bookUpdates.ApplyTo(updatedBookDto);
        if (!ModelState.IsValid || !TryValidateModel(updatedBookDto))
        {
            return BadRequest(ModelState);
        }

        var isValidEnum = updatedBookDto.Type!=null && Enum.IsDefined(typeof(BookTypes), updatedBookDto.Type);
        book.Title = updatedBookDto.Title;
        book.Author = updatedBookDto.Author;
        book.Type = isValidEnum ? (BookTypes)updatedBookDto.Type! : BookTypes.Unknown; //properties could be removed through patch as they are nullable in the dto
        book.PublicationYear = updatedBookDto.PublicationYear??1970; //properties could be removed through patch as they are nullable in the dto
        return Ok(booksService.UpdateBook(id, book).Result);
    }
}