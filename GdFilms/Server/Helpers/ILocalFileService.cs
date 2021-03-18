using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GdFilms.Server.Helpers
{
    public interface ILocalFileService
    {
        public Task Delete(string route, string containerName);
        public Task<string> Edit(byte[] content, string extension, string containerName, string currentFileRoute);
        public Task<string> Save(byte[] content, string extension, string containerName);
    }

    public class LocalFileService : ILocalFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task Delete(string route, string containerName)
        {
            var fileName = Path.GetFileName(route);
            string fileDirectory = Path.Combine(_environment.WebRootPath, containerName, fileName);
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            return Task.FromResult(0);
        }

        public async Task<string> Edit(byte[] content, string extension, string containerName, string currentFileRoute)
        {
            if (!string.IsNullOrEmpty(currentFileRoute))
            {
                await Delete(currentFileRoute, containerName);
            }
            return await Save(content, extension, containerName);
        }

        public async Task<string> Save(byte[] content, string extension, string containerName)
        {
            var fileName = $"{Guid.NewGuid()}.{extension}";
            string folder = Path.Combine(_environment.WebRootPath, containerName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string savedRoute = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(savedRoute, content);
            var currentUrl =    $"{_httpContextAccessor.HttpContext.Request.Scheme}://" +
                                $"{_httpContextAccessor.HttpContext.Request.Host}";
            var routeForDB = Path.Combine(currentUrl, containerName, fileName);
            return routeForDB;
        }
    }
}
