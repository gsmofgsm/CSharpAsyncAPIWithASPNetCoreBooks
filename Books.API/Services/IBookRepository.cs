﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.API.Services
{
    public interface IBookRepository
    {
        //IEnumerable<Entities.Book> GetBooks();

        //Entities.Book GetBook(Guid id);
        Task<IEnumerable<Entities.Book>> GetBooksAsync();  // naming convension ...Async if the method returns Task

        Task<Entities.Book> GetBookAsync(Guid id);
    }
}