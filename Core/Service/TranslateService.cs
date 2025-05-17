using snapwatch.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snapwatch.Core.Service
{
    public class TranslateService
    {
        private readonly Config _config;

        public TranslateService()
        {
            this._config = new Config();
        }

        public string RU_TO_EN(string text)
        {
            string url = $"{this._config.ReturnConfig().TRANSLATE_WWW_URL}?dl=en&text={text}";
        }
    }
}
