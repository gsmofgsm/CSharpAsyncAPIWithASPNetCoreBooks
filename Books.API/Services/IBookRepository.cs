﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.API.Services
{
    public interface IBookRepository
    {
        IEnumerable<Entities.Book> GetBooks();

        Entities.Book GetBook(Guid id);

        Task<IEnumerable<Entities.Book>> GetBooksAsync();  // naming convension ...Async if the method returns Task

        Task<Entities.Book> GetBookAsync(Guid id); // the suffix here tells the consumer that this is async

        Task<IEnumerable<Entities.Book>> GetBooksAsync(IEnumerable<Guid> booksId);

        void AddBook(Entities.Book bookToAdd);

        Task<bool> SaveChangesAsync();
    }
}
