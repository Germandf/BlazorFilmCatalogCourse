using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdFilms.Shared.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        public int AmountToShow { get; set; } = 10;
    }
}
