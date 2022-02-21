using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Shared.Models
{
    public class FilmActor
    {
        public int PersonId { get; set; }
        public int FilmId { get; set; }
        public Person Person { get; set; }
        public Film Film { get; set; }
        public string Character { get; set; }
        public int Order { get; set; }
    }
}
