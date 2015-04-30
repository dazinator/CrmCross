using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PortableCrmSdk
{
    public class SoapEndpointHelper
    {

        private Uri _Authority = null;
        //private string _Resource = null;   

        public SoapEndpointHelper(string orgServiceUrl, Version clientVersion)
        {
            if (clientVersion == null)
            {
                throw new ArgumentNullException("clientVersion");
            }

            OrgServiceUrl = orgServiceUrl;
            ClientVersion = clientVersion;

            string endpointUrl = string.Format("{0}/web?SdkClientVersion={1}", orgServiceUrl, clientVersion.ToString());
            SoapEndpoint = new Uri(endpointUrl);
        }

        public async Task<Uri> GetAuthority()
        {
            if (_Authority == null)
            {
                _Authority = await DiscoveryAuthority();
            }
            return _Authority;
        }

        /// <summary>
        /// Discover the authority for authentication.
        /// </summary>
        /// <param name="serviceUrl">The SOAP endpoint for a tenant organization.</param>
        /// <returns>The decoded authority URL.</returns>
        /// <remarks>The passed service URL string must contain the SdkClientVersion property.
        /// Otherwise, the discovery feature will not be available.</remarks>
        private async Task<Uri> DiscoveryAuthority()
        {
            // Use AuthenticationParameters to send a request to the organization's endpoint and
            // receive tenant information in the 401 challenge. 
            AuthenticationParameters parameters = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");

                //httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                // need to specify soap endpoint with client version,.
                using (HttpResponseMessage httpResponse = await httpClient.GetAsync(SoapEndpoint))
                {

                    parameters = await AuthenticationParameters.CreateFromUnauthorizedResponseAsync(httpResponse);
                    Uri authorityUri = new Uri(parameters.Authority);
                    return authorityUri;
                    // For phone, we dont need oauth2/authorization part.
                    // AuthorityUrl = System.Net.WebUtility.UrlDecode(httpResponse.Headers.GetValues("WWW-Authenticate").FirstOrDefault().Split('=')[1]).Replace("oauth2/authorize", "");

                }

            }

            // If the expected response is returned, this code should not execute.
            // throw new AdalException("unauthorized_response_expected", "Unauthorized http response (status code 401) was expected");

            // Return the authority URL.
            // _Resource = parameters.Resource;

        }

        public async Task<AuthenticationResult> AuthenticateAsync(string resource, string clientId, UserCredential credential)
        {
            var auth = GetAuthority();
            // Obtain an authentication token to access the web service.
            var authenticationContext = new AuthenticationContext(auth.ToString(), false);
            // AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resource, clientId);

            AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resource, clientId, credential);
            return result;
        }

        public string OrgServiceUrl { get; set; }
        public Uri SoapEndpoint { get; set; }
        public Version ClientVersion { get; set; }

    }
}
