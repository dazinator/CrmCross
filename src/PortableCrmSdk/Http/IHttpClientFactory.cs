using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CrmCross.Http
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient();
    }
}
