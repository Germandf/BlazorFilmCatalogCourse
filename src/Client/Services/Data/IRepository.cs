using BlazorFilmCatalogCourse.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Client.Services.Data
{
    public interface IRepository
    {
        public Task<HttpResponseWrapper<T>> Get<T>(string url);
        public Task<HttpResponseWrapper<object>> Post<T>(string url, T obj);
        public Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T obj);
        public Task<HttpResponseWrapper<object>> Put<T>(string url, T obj);
        public Task<HttpResponseWrapper<object>> Delete(string url);
    }

    public class Repository : IRepository
    {
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions _defaultOptionsJSON => new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public Repository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseWrapper<T>> Get<T> (string url)
        {
            var httpResponse = await _httpClient.GetAsync(url);
            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await DeserealizeResponse<T>(httpResponse, _defaultOptionsJSON);
                return new HttpResponseWrapper<T>(response, false, httpResponse);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, true, httpResponse);
            }
        }

        public async Task<HttpResponseWrapper<object>> Post<T> (string url, T obj)
        {
            var json = JsonSerializer.Serialize(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync(url, content);
            return new HttpResponseWrapper<object>(null, !httpResponse.IsSuccessStatusCode, httpResponse);
        }

        public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T obj)
        {
            var json = JsonSerializer.Serialize(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync(url, content);
            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await DeserealizeResponse<TResponse>(httpResponse, _defaultOptionsJSON);
                return new HttpResponseWrapper<TResponse>(response, false, httpResponse);
            }
            else
            {
                return new HttpResponseWrapper<TResponse>(default, true, httpResponse);
            }
        }

        public async Task<HttpResponseWrapper<object>> Put<T>(string url, T obj)
        {
            var json = JsonSerializer.Serialize(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PutAsync(url, content);
            return new HttpResponseWrapper<object>(null, !httpResponse.IsSuccessStatusCode, httpResponse);
        }

        public async Task<HttpResponseWrapper<object>> Delete(string url)
        {
            var httpResponse = await _httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, !httpResponse.IsSuccessStatusCode, httpResponse);
        }

        private async Task<T> DeserealizeResponse<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseString, jsonSerializerOptions);
        }
    }
}
