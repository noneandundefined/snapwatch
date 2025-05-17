using snapwatch.Core.Core;
using snapwatch.Core.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<string> RU_TO_EN(string text)
        {
            string url = $"{this._config.ReturnConfig().TRANSLATE_WWW_URL}?dl=en&text={Uri.EscapeDataString(text)}";

            HttpResponseMessage response = await this._http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            Stream stream = await response.Content.ReadAsStreamAsync();

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            TranslateModel translated = await System.Text.Json.JsonSerializer.DeserializeAsync<TranslateModel>(stream, options);

            response.Dispose();
            stream.Dispose();

            return translated.DestinationText;
        }

        public bool IS_EN(string text, double threshold = 0.9)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            ushort totalLetters = 0;
            ushort latinLetters = 0;

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    totalLetters++;

                    if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    {
                        latinLetters++;
                    }
                }
            }

            if (totalLetters == 0)
            {
                return false;
            }

            double ratio = (double)latinLetters / totalLetters;
            return ratio >= threshold;
        }
    }
}
