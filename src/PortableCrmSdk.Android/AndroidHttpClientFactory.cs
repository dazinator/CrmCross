using CrmCross.Http;
using System.Net.Http;
using ModernHttpClient;

namespace PortableCrmSdk.Http
{
    public class AndroidHttpClientFactory : IHttpClientFactory
    {       
        public HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient(new NativeMessageHandler());
            return httpClient;
        }
    }

  
}