using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorFilmCatalogCourse.Shared.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Debe insertar un nombre")]
        public string Name { get; set; }
        public List<FilmGenre> filmGenres { get; set; }
    }
}
