using Books.API.Contexts;
using Books.API.Entities;
using Books.API.ExternalModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Books.API.Services
{
    public class BookRepository : IBookRepository, IDisposable
    {
        private BooksContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private bool disposedValue;

        public BookRepository(BooksContext context,
            IHttpClientFactory httpClientFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Book> GetBookAsync(Guid id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:02';");
            //return _context.Books.ToList();
            return await _context.Books.Include(b => b.Author).ToListAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   if (_context != null)
                    {
                        _context.Dispose();
                        _context = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BookRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Book> GetBooks()
        {
            _context.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:02';");
            return _context.Books.Include(b => b.Author).ToList();
        }

        public Book GetBook(Guid id)
        {
            return _context.Books.Include(b => b.Author).FirstOrDefault(b => b.Id == id);
        }

        public void AddBook(Book bookToAdd) // add to context is not async
        {
            if (bookToAdd == null)
            {
                throw new ArgumentNullException(nameof(bookToAdd));
            }

            _context.Add(bookToAdd);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> booksId)
        {
            return await _context.Books.Where(b => booksId.Contains(b.Id))
                .Include(b => b.Author).ToListAsync();
        }

        public async Task<BookCover> GetBookCoverAsync(string coverId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // pass through a dummy name
            var response = await httpClient
                .GetAsync($"http://localhost:21798/api/bookcovers/{coverId}");

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<BookCover>(
                    await response.Content.ReadAsStringAsync(), // this is a I/O operation, buffering, transfering
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,  // json format default with first letter small, while our Model has property name first letter capital
                    });
            }

            return null;
        }

        public async Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var bookCovers = new List<BookCover>();

            // create a list of fake bookcovers
            var bookCoverUrls = new[]
            {
                $"http://localhost:21798/api/bookcovers/{bookId}-dummycover1",
                $"http://localhost:21798/api/bookcovers/{bookId}-dummycover2",
                $"http://localhost:21798/api/bookcovers/{bookId}-dummycover3",
                $"http://localhost:21798/api/bookcovers/{bookId}-dummycover4",
                $"http://localhost:21798/api/bookcovers/{bookId}-dummycover5",
            };

            // create the tasks
            var downloadBookCoverTasksQuery =
                from bookCoverUrl
                in bookCoverUrls
                select DownloadBookCoverAsync(httpClient, bookCoverUrl);

            // start the tasks
            var downloadBookCoverTasks = downloadBookCoverTasksQuery.ToList();

            return await Task.WhenAll(downloadBookCoverTasks);  // WhenAll will put them in order again!

            //foreach (var bookCoverUrl in bookCoverUrls)
            //{
            //    var response = await httpClient.GetAsync(bookCoverUrl);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        bookCovers.Add(JsonSerializer.Deserialize<BookCover>(
            //            await response.Content.ReadAsStringAsync(),
            //            new JsonSerializerOptions
            //            {
            //                PropertyNameCaseInsensitive = true,
            //            }));
            //    }
            //}

            //return bookCovers;
        }

        private async Task<BookCover> DownloadBookCoverAsync(
            HttpClient httpClient, string bookCoverUrl)
        {
            var response = await httpClient.GetAsync(bookCoverUrl); // they come in in order

            if (response.IsSuccessStatusCode)
            {
                var bookCover = JsonSerializer.Deserialize<BookCover>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
                return bookCover; // they get processed when ever they can, so disordered
            }

            return null;
        }
    }
}
