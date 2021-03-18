using AutoMapper;
using GdFilms.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GdFilms.Server.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Person, Person>()
                .ForMember(x => x.Photo, option => option.Ignore());
            CreateMap<Film, Film>()
                .ForMember(x => x.Image, option => option.Ignore());
        }
    }
}
