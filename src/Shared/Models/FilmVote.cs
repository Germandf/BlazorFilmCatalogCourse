using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Shared.Models
{
    public class FilmVote
    {
        public int Id { get; set; }
        public int Vote { get; set; }
        public DateTime VoteDate { get; set; }
        public int FilmId { get; set; }
        public Film Film { get; set; }
        public string UserId { get; set; }
    }
}
