using BlazorFilmCatalogCourse.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Client.Auth
{
    public interface ILoginService
    {
        public Task Login(UserTokenDTO userTokenDTO);
        public Task Logout();
        public Task ManageTokenRenew();
    }
}
