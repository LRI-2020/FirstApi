using FirstApi.Models;
using FirstApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : Controller
{
    private readonly BooksService booksService;

    public BooksController(BooksService booksService)
    {
        this.booksService = booksService;
    }
    // GET
    [HttpGet]
    public IEnumerable<Book> Get()
    {
        return booksService.GetBooks().Result;
    }
}