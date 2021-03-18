using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GdFilms.Server.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParamsOnResponse<T>(
            this HttpContext context, 
            IQueryable<T> queryable, 
            int amountToShow)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            double counting = await queryable.CountAsync();
            double totalPages = Math.Ceiling(counting / amountToShow);
            context.Response.Headers.Add("counting", counting.ToString());
            context.Response.Headers.Add("totalPages", totalPages.ToString());
        }
    }
}
