using AutoMapper;
using Books.API.Filters;
using Books.API.ModelBinders;
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
    [Route("api/bookcollections")]
    [ApiController]
    [BooksResultFilter]
    public class BookCollectionsController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookCollectionsController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository ?? 
                throw new ArgumentNullException(nameof(bookRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        // api/bookcollections/(id1,id2,...)
        [HttpGet("({bookIds})", Name = "GetBookCollection")]
        public async Task<IActionResult> GetBookCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> bookIds)
        {
            var bookEntities = await _bookRepository.GetBooksAsync(bookIds);

            if (bookIds.Count() != bookEntities.Count())
            {
                return NotFound();
            }

            return Ok(bookEntities);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookCollection(
            IEnumerable<BookForCreation> bookCollection)
        {
            var bookEntities = _mapper.Map<IEnumerable<Entities.Book>>(bookCollection);

            foreach (var bookEntity in bookEntities)
            {
                _bookRepository.AddBook(bookEntity);
            }

            await _bookRepository.SaveChangesAsync();

            var booksToReturn = await _bookRepository.GetBooksAsync(
                bookEntities.Select(b => b.Id).ToList());

            var bookIds = string.Join(",", booksToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetBookCollection", 
                new { bookIds },
                booksToReturn);
        }
    }
}
