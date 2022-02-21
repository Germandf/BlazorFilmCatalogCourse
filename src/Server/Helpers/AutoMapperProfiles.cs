using AutoMapper;
using BlazorFilmCatalogCourse.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Server.Helpers
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
