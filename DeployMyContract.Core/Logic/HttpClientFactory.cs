using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DeployMyContract.Core.Logic
{
    public static class HttpClientFactory
    {
        public static HttpClient CreateFirefox(Uri referer)
            => new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        MediaTypeWithQualityHeaderValue.Parse("text/html"),
                        MediaTypeWithQualityHeaderValue.Parse("application/xhtml+xml"),
                        MediaTypeWithQualityHeaderValue.Parse("application/xml;q=0.9"),
                        MediaTypeWithQualityHeaderValue.Parse("*/*;q=0.8")
                    },
                    AcceptLanguage =
                    {
                        StringWithQualityHeaderValue.Parse("ru-RU"),
                        StringWithQualityHeaderValue.Parse("ru;q=0.8"),
                        StringWithQualityHeaderValue.Parse("en-US;q=0.5"),
                        StringWithQualityHeaderValue.Parse("en;q=0.3")
                    },
                    UserAgent =
                    {
                        ProductInfoHeaderValue.Parse("Mozilla/5.0"),
                        ProductInfoHeaderValue.Parse("(Windows NT 6.1; WOW64; rv:54.0)"),
                        ProductInfoHeaderValue.Parse("Gecko/20100101"),
                        ProductInfoHeaderValue.Parse("Firefox/54.0")
                    },
                    Referrer = referer
                }
            };
    }
}
