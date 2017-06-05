using CrmCross.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{

    /// <summary>
    /// Provides authentication tokens using ADAL.
    /// </summary>
    public class AdalAuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private AuthenticationContext _authContext = null;
        private readonly static AuthenticationHeaderValue _authHeader = new AuthenticationHeaderValue("Bearer");
        private IAuthenticationDetailsProvider _authenticationDetailsProvider;
        private IAuthenticationTokenResult _LastToken;
        private IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="authenticationDetailsProvider">Provides the authentication parameters for token aquisition.</param>
        /// <param name="httpClientFactory">Provides the HTTP client necessary for making HTTP requests for token aquisition.</param>
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

                    var response = await httpClient.GetAsync(dynamicsInfo.AuthenticationEndpointUrl).ConfigureAwait(false);
                    using (response)
                    {
                        parameters = await AuthenticationParameters.CreateFromUnauthorizedResponseAsync(response);
                        Uri authorityUri = new Uri(parameters.Authority);
                        _authContext = new AuthenticationContext(authorityUri.ToString(), false);
                        // For phone, we dont need oauth2/authorization part.
                        // AuthorityUrl = System.Net.WebUtility.UrlDecode(httpResponse.Headers.GetValues("WWW-Authenticate").FirstOrDefault().Split('=')[1]).Replace("oauth2/authorize", "");
                    }
                }
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

        /// <summary>
        /// Returns the last token that was successfully aquired.
        /// </summary>
        /// <returns></returns>
        public IAuthenticationTokenResult GetLastToken()
        {
            return _LastToken;
        }

        /// <summary>
        /// Returns an authentication token. If no user credentials are currently set, this will attempt to redirect the user to a sign in page. If user credentials are set,
        /// it will either return the token from the cache, or it will aquire a new token based on those credentials.
        /// </summary>
        /// <returns></returns>
        public async Task<IAuthenticationTokenResult> GetAuthenticateTokenAsync()
        {
            // Obtain an authentication token to access the web service.
            try
            {
                bool continueOnCapturedContext = _authContext == null;
                var authenticationContext = _authContext ?? await GetAuthenticationContext().ConfigureAwait(false);
                var clientAppDetails = GetClientDetails();
                var serverDetails = GetCrmServerDetails();
                var resource = serverDetails.CrmWebsiteUrl;

                UsernamePasswordCredential credentials = GetUserCredentials();
                AuthenticationResult result = null;

                if (credentials != null)
                {
                    UserCredential userCredential = credentials.UserCredential;
                    result = await authenticationContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, userCredential).ConfigureAwait(continueOnCapturedContext);
                }
                else
                {
                    var platform = GetPlatform();
                    if (platform == null)
                    {
                        throw new Exception("No token can be obtained because no UserCredential or Platform was provided.");
                    }
                    var platformParams = platform.PlatformParameters;
                    result = await authenticationContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, clientAppDetails.RedirectUri, platformParams).ConfigureAwait(continueOnCapturedContext);
                }

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

        #endregion

    }

}
