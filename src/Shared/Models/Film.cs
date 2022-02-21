using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorFilmCatalogCourse.Shared.Models
{
    public class Film
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool OnBillboard { get; set; }
        public string Trailer { get; set; }
        [Required]
        public DateTime? ReleaseDate { get; set; }
        public string Image { get; set; }
        public List<FilmGenre> FilmGenres { get; set; } = new List<FilmGenre>();
        public List<FilmActor> FilmActors { get; set; }
        public string ShortTitle 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    return null;
                }
                if (Title.Length > 60)
                {
                    return Title.Substring(0, 60) + "...";
                }
                else
                {
                    return Title;
                }
            }
        }
    }
}
