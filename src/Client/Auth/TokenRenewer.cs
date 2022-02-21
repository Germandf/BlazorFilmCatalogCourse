using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorFilmCatalogCourse.Client.Auth
{
    public class TokenRenewer : IDisposable
    {
        private Timer _timer;
        private ILoginService _loginService;

        public TokenRenewer(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Start()
        {
            _timer = new Timer();
            _timer.Interval = 1000 * 60 * 4; // 4 minutes
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _loginService.ManageTokenRenew();
        }
    }
}
