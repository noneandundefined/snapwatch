using DotNetEnv;
using System.Net;

namespace snapwatch.Core.Service
{
    public class HttpConfig
    {
        private readonly string _proxyAddress;
        private readonly string _proxyPort;
        private readonly string _proxyUsername;
        private readonly string _proxyPassword;

        public HttpConfig()
        {
            this._proxyAddress = Env.GetString("PROXY_ADDRESS");
            this._proxyPort = Env.GetString("PROXY_PORT");
            this._proxyUsername = Env.GetString("PROXY_USERNAME");
            this._proxyPassword = Env.GetString("PROXY_PASSWORD");
        }

        public WebProxy GetProxy()
        {
            string proxyUrl = $"http://{this._proxyAddress}:{this._proxyPort}";

            WebProxy proxy = new(proxyUrl)
            {
                Credentials = new NetworkCredential(this._proxyUsername, this._proxyPassword)
            };

            return proxy;
        }
    }
}
