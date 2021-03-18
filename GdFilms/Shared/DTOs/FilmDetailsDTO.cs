using GdFilms.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdFilms.Shared.DTOs
{
    public class FilmDetailsDTO
    {
        public Film Film { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Person> People { get; set; }
        public double AverageVote { get; set; }
        public int UserVote { get; set; }
    }
}
