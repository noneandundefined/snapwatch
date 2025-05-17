using snapwatch.Core.Core;
using snapwatch.Core.Models;
using System;
using System.IO;
using System.Net.Http;

namespace snapwatch.Core.Service
{
    public class TranslateService
    {
        private readonly HttpClient _http;
        private readonly Config _config;

        public TranslateService()
        {
            this._http = new HttpClient();
            this._config = new Config();
        }

        public async string RU_TO_EN(string text)
        {
            string url = $"{this._config.ReturnConfig().TRANSLATE_WWW_URL}?dl=en&text={Uri.EscapeDataString(text)}";

            HttpResponseMessage response = await this._http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            Stream stream = await response.Content.ReadAsStreamAsync();

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            TranslateModel translated = await System.Text.Json.JsonSerializer.Deserialize<TranslateModel>(stream, options);
        }
    }
}
