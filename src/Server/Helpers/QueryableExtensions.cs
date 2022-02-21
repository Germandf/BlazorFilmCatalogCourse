using BlazorFilmCatalogCourse.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Server.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.AmountToShow)
                .Take(paginationDTO.AmountToShow);
        }
    }
}
