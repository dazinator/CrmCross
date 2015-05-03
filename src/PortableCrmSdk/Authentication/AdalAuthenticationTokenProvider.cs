using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{
    public class AdalAuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private AuthenticationContext _authContext = null;
        private readonly static AuthenticationHeaderValue _authHeader = new AuthenticationHeaderValue("Bearer");

        private AdalClientApplicationDetails _clientAppInfo;
        private CrmServerDetails _dynamicsWebServerInfo;
        private IAuthenticationTokenResult _LastToken;

        public AdalAuthenticationTokenProvider(AdalClientApplicationDetails clientAppInfo,
                                               CrmServerDetails dynamicsWebServerInfo)
        {
            if (clientAppInfo == null)
            {
                throw new ArgumentNullException("clientAppInfo");
            }
            if (dynamicsWebServerInfo == null)
            {
                throw new ArgumentNullException("dynamicsWebServerInfo");
            }
            _clientAppInfo = clientAppInfo;
            _dynamicsWebServerInfo = dynamicsWebServerInfo;
        }

        public async Task<AuthenticationContext> GetAuthenticationContext()
        {
            if (_authContext == null)
            {
                var authority = await DiscoverAuthority();
                _authContext = new AuthenticationContext(authority.ToString(), false);
            }
            return _authContext;
        }

        /// <summary>
        /// Discover the authority for authentication.
        /// </summary>
        /// <param name="serviceUrl">The SOAP endpoint for a tenant organization.</param>
        /// <returns>The decoded authority URL.</returns>
        /// <remarks>The passed service URL string must contain the SdkClientVersion property.
        /// Otherwise, the discovery feature will not be available.</remarks>
        private async Task<Uri> DiscoverAuthority()
        {
            // Use AuthenticationParameters to send a request to the organization's endpoint and
            // receive tenant information in the 401 challenge. 
            AuthenticationParameters parameters = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = _authHeader;
                //httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                // need to specify soap endpoint with client version,.
                var dynamicsInfo = GetCrmServerDetails();
                using (HttpResponseMessage httpResponse = await httpClient.GetAsync(dynamicsInfo.AuthenticationEndpointUrl))
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

        public async Task<IAuthenticationTokenResult> ExecuteAuthenticationRequest(Func<AuthenticationContext, Task<AuthenticationResult>> authenticate)
        {
            // Obtain an authentication token to access the web service.
            var authenticationContext = _authContext ?? await GetAuthenticationContext();

            try
            {
                var result = await authenticate(authenticationContext);
                var authTokenResult = new AuthenticationTokenResult(true, result.AccessToken);
                _LastToken = authTokenResult;
                return authTokenResult;
            }
            catch (Exception e)
            {
                var exResult = new AuthenticationTokenResult(false, String.Empty);
                exResult.Exception = e;
                return exResult;
            }
        }

        protected virtual ClientApplicationDetails GetClientDetails()
        {
            return _clientAppInfo;
        }

        protected virtual CrmServerDetails GetCrmServerDetails()
        {
            return _dynamicsWebServerInfo;
        }

        #region IAuthenticationProvider

        public async Task<IAuthenticationTokenResult> GetAuthenticationTokenAsync(string username, string password)
        {
            var result = await this.ExecuteAuthenticationRequest(authContext =>
                {
                    var clientAppDetails = GetClientDetails();
                    var serverDetails = GetCrmServerDetails();
                    var resource = serverDetails.CrmWebsiteUrl;
                    var userCredential = new UserCredential(username, password);
                    return authContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, userCredential);
                });
            return result;
        }

        public async Task<IAuthenticationTokenResult> GetAuthenticateTokenAsync()
        {
            var result = await this.ExecuteAuthenticationRequest(authContext =>
            {
                var clientAppDetails = GetClientDetails();
                var serverDetails = GetCrmServerDetails();
                var resource = serverDetails.CrmWebsiteUrl;
                return authContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, clientAppDetails.RedirectUri, _clientAppInfo.PlatformParams);
            });

            return result;
        }

        public IAuthenticationTokenResult GetLastToken()
        {
            return _LastToken;
        }

        #endregion





    }


}
