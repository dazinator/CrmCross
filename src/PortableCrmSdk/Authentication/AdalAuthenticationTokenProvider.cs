using CrmCross.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{ 
    
    public class AdalAuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private AuthenticationContext _authContext = null;
        private readonly static AuthenticationHeaderValue _authHeader = new AuthenticationHeaderValue("Bearer");
        private IAuthenticationDetailsProvider _authenticationDetailsProvider;
        private IAuthenticationTokenResult _LastToken;
        private IHttpClientFactory _httpClientFactory;

        public AdalAuthenticationTokenProvider(IAuthenticationDetailsProvider authenticationDetailsProvider, IHttpClientFactory httpClientFactory)
        {
            if (authenticationDetailsProvider == null)
            {
                throw new ArgumentNullException("authenticationDetailsProvider");
            }

            if (httpClientFactory == null)
            {
                throw new ArgumentNullException("httpClientFactory");
            }

            _httpClientFactory = httpClientFactory;
            _authenticationDetailsProvider = authenticationDetailsProvider;          
        }

        private async Task<AuthenticationContext> GetAuthenticationContext()
        {
            if (_authContext == null)
            {
                AuthenticationParameters parameters = null;
                using (HttpClient httpClient = _httpClientFactory.GetHttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = _authHeader;
                    //httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                    // need to specify soap endpoint with client version,.
                    var dynamicsInfo = GetCrmServerDetails();

                    using (HttpResponseMessage httpResponse = await httpClient.GetAsync(dynamicsInfo.AuthenticationEndpointUrl))
                    {
                        parameters = await AuthenticationParameters.CreateFromUnauthorizedResponseAsync(httpResponse);
                        Uri authorityUri = new Uri(parameters.Authority);
                        _authContext = new AuthenticationContext(authorityUri.ToString(), false);
                        // For phone, we dont need oauth2/authorization part.
                        // AuthorityUrl = System.Net.WebUtility.UrlDecode(httpResponse.Headers.GetValues("WWW-Authenticate").FirstOrDefault().Split('=')[1]).Replace("oauth2/authorize", "");
                    }
                }
                //var authority = await DiscoverAuthority().ConfigureAwait(false);

            }
            return _authContext;
        }        

        protected virtual ClientApplicationDetails GetClientDetails()
        {
            return _authenticationDetailsProvider.ClientApplicationDetails;
        }

        protected virtual CrmServerDetails GetCrmServerDetails()
        {
            return _authenticationDetailsProvider.CrmServerDetails;
        }

        protected virtual UsernamePasswordCredential GetUserCredentials()
        {
            return _authenticationDetailsProvider.UserCredentials;
        }

        protected virtual Platform GetPlatform()
        {
            return _authenticationDetailsProvider.Platform;
        }

        #region IAuthenticationProvider  

        public IAuthenticationTokenResult GetLastToken()
        {
            return _LastToken;
        }

        #endregion

        public async Task<IAuthenticationTokenResult> GetAuthenticateTokenAsync()
        {
            // Obtain an authentication token to access the web service.
            try
            {
                var authenticationContext = _authContext ?? await GetAuthenticationContext();
                var clientAppDetails = GetClientDetails();
                var serverDetails = GetCrmServerDetails();
                var resource = serverDetails.CrmWebsiteUrl;

                //    var userCredential = new UserCredential(username, password);

                UsernamePasswordCredential credentials = GetUserCredentials();
                UserCredential userCredential = credentials.UserCredential;

                AuthenticationResult result = null;

                if (credentials != null)
                {
                    result = await authenticationContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, userCredential);
                }
                else
                {
                    var platform = GetPlatform();
                    if (platform == null)
                    {
                        throw new Exception("No token can be obtained because no UserCredential or Platform was provided.");
                    }
                    var platformParams = platform.PlatformParameters;
                    result = await authenticationContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, clientAppDetails.RedirectUri, platformParams);
                }

                // return authContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, userCredential);

                //  var result = await authenticate(authenticationContext);
                var authTokenResult = new AuthenticationTokenResult(result);
                _LastToken = authTokenResult;
                return authTokenResult;
            }
            catch (Exception e)
            {
                var exResult = new AuthenticationTokenResult(null);
                exResult.Exception = e;
                return exResult;
            }
        }

    }

}
