using GdFilms.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdFilms.Shared.DTOs
{
    public class HomePageDTO
    {
        public List<Film> FilmsOnBillboard { get; set; }
        public List<Film> UpcomingReleases { get; set; }
    }
}
