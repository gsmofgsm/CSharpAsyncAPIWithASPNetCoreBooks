using Books.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.API.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository ??
                throw new ArgumentNullException(nameof(bookRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks() // controller actions aren't meant for consumers, so no suffix Async here
        {
            var bookEntities = await _bookRepository.GetBooksAsync();
            return Ok(bookEntities);
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBook(Guid bookId)
        {
            var bookEntity = await _bookRepository.GetBookAsync(bookId);
            if (bookEntity == null)
            {
                return NotFound();
            }
            return Ok(bookEntity);
        }
    }
}
