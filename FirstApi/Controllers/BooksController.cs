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
    private readonly IBooksService booksService;

    public BooksController(IBooksService booksService)
    {
        this.booksService = booksService;
    }

    /// <summary>
    /// Return all the books
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
    {
        var books = await booksService.GetBooksAsync();
        var res = books as Book[] ?? books.ToArray();
        if (!res.Any())
            return NotFound();
        return Ok(res);
    }

    /// <summary>
    /// Return a book based on its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Route("/book/{id}")]
    [HttpGet]
    public async Task<ActionResult<Book>> GetBookByIdAsync(int id)
    {
        var book = await booksService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound(new ProblemDetails() { Title = $"Book with Id {id} not found" });
        return Ok(book);
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    /// <param name="bookInput"></param>
    /// <returns></returns>
    [Route("/book")]
    [HttpPost]
    public async Task<ActionResult<string>> CreateBookAsync(BookDto bookInput)
    {
        if (bookInput.Type == null) return Ok(await booksService.CreateBookAsync(bookInput.Title, bookInput.Author, null, bookInput.PublicationYear, bookInput.Ratings));
        var isValidType = bookInput.Type != null && Enum.IsDefined(typeof(BookTypes), bookInput.Type);
        return Ok(await booksService.CreateBookAsync(bookInput.Title, bookInput.Author, isValidType ? (BookTypes)bookInput.Type! : null, bookInput.PublicationYear, bookInput.Ratings));
    }
/// <summary>
///     Update an existing book
/// </summary>
/// <returns></returns>
    [Route("/book/{id}")]
    [HttpPut]
    public async Task<ActionResult<string>> UpdateBookAsync(int id, BookDto bookInput)
    {
        var originalBook = await booksService.GetBookByIdAsync(id);
        if (originalBook == null)
            return NotFound();
        //If nullable property is not passed, kept existing value to avoid a user loosing data without explicitly asked for it
        var newType = bookInput.Type == null || !Enum.IsDefined(typeof(BookTypes), bookInput.Type) ? originalBook.Type : (BookTypes)bookInput.Type;
        var newDate = bookInput.PublicationYear ?? originalBook.PublicationYear;
        originalBook.Author = bookInput.Author;
        originalBook.Title = bookInput.Title;
        originalBook.Ratings = bookInput.Ratings;
        originalBook.PublicationYear = newDate;
        originalBook.Type = newType;
        return Ok(await booksService.UpdateBookAsync(id, originalBook));
    }

    /// <summary>
    /// Delete all the books
    /// </summary>
    /// <returns></returns>
    [HttpDelete("all")] //Will listen to /books/all
    public async Task<ActionResult<int>> DeleteAllAsync()
    {
        return Ok(await booksService.DeleteAllAsync());
    }

    /// <summary>
    /// Delete a book
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Route("/book/{id}")]
    [HttpDelete] //Will listen to /books/{id}
    public async Task<ActionResult<bool>> DeleteByIdAsync(int id)
    {
        return Ok(await booksService.DeleteByIdAsync(id));
    }

    /// <summary>
    /// Update a specific property of a book
    /// </summary>
    /// <param name="id"></param>
    /// <param name="bookUpdates"></param>
    /// <returns></returns>
    [Route("/book/{id}")]
    [HttpPatch]
    public async Task<ActionResult> PatchBookAsync(int id, JsonPatchDocument<BookDto> bookUpdates)
    {
        var book = await booksService.GetBookByIdAsync(id);
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
        return Ok(await booksService.UpdateBookAsync(id, book));
    }
}