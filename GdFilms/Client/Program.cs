using GdFilms.Client.Auth;
using GdFilms.Client.Helpers;
using GdFilms.Client.Services.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GdFilms.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            ConfigureServices(builder.Services);

            await builder.Build().RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IShowMessages, ShowMessages>();
            services.AddAuthorizationCore();
            services.AddScoped<JWTAuthProvider>();
            services.AddScoped<AuthenticationStateProvider, JWTAuthProvider>(
                provider => provider.GetRequiredService<JWTAuthProvider>());
            services.AddScoped<ILoginService, JWTAuthProvider>(
                provider => provider.GetRequiredService<JWTAuthProvider>());
            services.AddScoped<TokenRenewer>();
        }
    }
}
