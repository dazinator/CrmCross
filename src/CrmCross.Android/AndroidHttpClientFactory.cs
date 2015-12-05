using CrmCross.Http;
using System.Net.Http;
using ModernHttpClient;

namespace CrmCross.Http
{
    /// <summary>
    /// An <see cref="IHttpClientFactory"/> that returns a HttpClient leveraging ModernHttpClient for faster performance.
    /// </summary>
    public class AndroidHttpClientFactory : IHttpClientFactory
    {       
        public HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient(new NativeMessageHandler());
            return httpClient;
        }
    }

  
}