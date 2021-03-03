﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.API.Filters
{
    public class BookWithCoversResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultFromAction = context.Result as ObjectResult;
            if (resultFromAction?.Value == null
                || resultFromAction.StatusCode < 200
                || resultFromAction.StatusCode >= 300)
            {
                await next();
                return;
            }

            //var (book, bookCovers) = ((Entities.Book book, 
            //    IEnumerable<ExternalModels.BookCover> bookCovers))resultFromAction.Value;

            //var temp = ((Entities.Book book,
            //    IEnumerable<ExternalModels.BookCover> bookCovers))resultFromAction.Value;

            var (book, bookCovers) = ((Entities.Book,
                IEnumerable<ExternalModels.BookCover>))resultFromAction.Value;

            await next();
        }
    }
}