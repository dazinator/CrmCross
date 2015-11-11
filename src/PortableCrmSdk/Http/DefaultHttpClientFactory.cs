using System.Net.Http;

namespace CrmCross.Http
{
    /// <summary>
    /// An <see cref="IHttpClientFactory"/> that returns a default HttpClient.
    /// </summary>
    public class DefaultHttpClientFactory : IHttpClientFactory
    {
        public HttpClient GetHttpClient()
        {
            return new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip });
        }
    }
}
