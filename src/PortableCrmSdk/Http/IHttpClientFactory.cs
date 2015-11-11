using System.Net.Http;

namespace CrmCross.Http
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient();
    }  

}
