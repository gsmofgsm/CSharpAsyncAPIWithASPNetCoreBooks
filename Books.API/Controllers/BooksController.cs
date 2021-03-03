using AutoMapper;
using Books.API.Filters;
using Books.API.Models;
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
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository ??
                throw new ArgumentNullException(nameof(bookRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [BooksResultFilter]
        public async Task<IActionResult> GetBooks() // controller actions aren't meant for consumers, so no suffix Async here
        {
            var bookEntities = await _bookRepository.GetBooksAsync();
            return Ok(bookEntities);
        }

        [HttpGet("{bookId}", Name = "GetBook")]
        [BookResultFilter]
        public async Task<IActionResult> GetBook(Guid bookId)
        {
            var bookEntity = await _bookRepository.GetBookAsync(bookId);
            if (bookEntity == null)
            {
                return NotFound();
            }

            var bookCover = await _bookRepository.GetBookCoverAsync("dummycover");

            return Ok(bookEntity);
        }

        [HttpPost]
        [BookResultFilter]
        public async Task<IActionResult> CreateBook(BookForCreation bookForCreation)
        {
            var bookEntity = _mapper.Map<Entities.Book>(bookForCreation);
            _bookRepository.AddBook(bookEntity);
            await _bookRepository.SaveChangesAsync();

            // Fetch (refetch) the book from the data store, including the author
            await _bookRepository.GetBookAsync(bookEntity.Id);

            return CreatedAtRoute(
                "GetBook",
                new { bookId = bookEntity.Id }, // pass through an anonymous object
                bookEntity);
        }
    }
}
