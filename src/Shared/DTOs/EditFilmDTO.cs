using BlazorFilmCatalogCourse.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Shared.DTOs
{
    public class EditFilmDTO
    {
        public Film Film { get; set; }
        public List<Person> People { get; set; }
        public List<Genre> SelectedGenres { get; set; }
        public List<Genre> NotSelectedGenres { get; set; }
    }
}
