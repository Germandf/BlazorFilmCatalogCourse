using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdFilms.Shared.DTOs
{
    public class UserTokenDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
