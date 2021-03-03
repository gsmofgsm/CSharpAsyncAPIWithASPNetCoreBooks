using Books.API.Filters;
using Books.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.API.Controllers
{
    [Route("api/synchronousbooks")]
    [ApiController]
    public class SynchronousBooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public SynchronousBooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [BooksResultFilter]
        public IActionResult GetBooks()
        {
            var bookEntities = _bookRepository.GetBooks();
            return Ok(bookEntities);
        }
    }
}
