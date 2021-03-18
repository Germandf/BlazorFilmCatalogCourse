using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdFilms.Shared.Models
{
    public class FilmGenre
    {
        public int FilmId { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public Film Film { get; set; }
    }
}
