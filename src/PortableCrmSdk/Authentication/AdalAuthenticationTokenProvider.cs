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

    public class UsernamePasswordCredential
    {
        private string _password;

        public UsernamePasswordCredential(string username, string password)
        {
            _password = password;
            UserCredential = new UserCredential(username, password);
        }

        public string Username
        {
            get
            {
                return UserCredential.UserName;
            }
            set
            {
                UserCredential = new UserCredential(value, _password);
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                UserCredential = new UserCredential(UserCredential.UserName, value);
                _password = value;
            }
        }

        public UserCredential UserCredential { get; set; }

    }

    public interface IAuthenticationDetailsProvider
    {
        UsernamePasswordCredential UserCredentials { get; set; }
        Platform Platform { get; }
        CrmServerDetails CrmServerDetails { get; }
        ClientApplicationDetails ClientApplicationDetails { get; }
    }


    public class AdalAuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private AuthenticationContext _authContext = null;
        private readonly static AuthenticationHeaderValue _authHeader = new AuthenticationHeaderValue("Bearer");
        private IAuthenticationDetailsProvider _authenticationDetailsProvider;
        private IAuthenticationTokenResult _LastToken;

        public AdalAuthenticationTokenProvider(IAuthenticationDetailsProvider authenticationDetailsProvider)
        {
            if (authenticationDetailsProvider == null)
            {
                throw new ArgumentNullException("authenticationDetailsProvider");
            }
            _authenticationDetailsProvider = authenticationDetailsProvider;          
        }

        private async Task<AuthenticationContext> GetAuthenticationContext()
        {
            if (_authContext == null)
            {
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
                        _authContext = new AuthenticationContext(authorityUri.ToString(), false);
                        // For phone, we dont need oauth2/authorization part.
                        // AuthorityUrl = System.Net.WebUtility.UrlDecode(httpResponse.Headers.GetValues("WWW-Authenticate").FirstOrDefault().Split('=')[1]).Replace("oauth2/authorize", "");
                    }
                }
                //var authority = await DiscoverAuthority().ConfigureAwait(false);

            }
            return _authContext;
        }

        ///// <summary>
        ///// Discover the authority for authentication.
        ///// </summary>
        ///// <param name="serviceUrl">The SOAP endpoint for a tenant organization.</param>
        ///// <returns>The decoded authority URL.</returns>
        ///// <remarks>The passed service URL string must contain the SdkClientVersion property.
        ///// Otherwise, the discovery feature will not be available.</remarks>
        //private async Task<Uri> DiscoverAuthority()
        //{
        //    // Use AuthenticationParameters to send a request to the organization's endpoint and
        //    // receive tenant information in the 401 challenge. 


        //    // If the expected response is returned, this code should not execute.
        //    // throw new AdalException("unauthorized_response_expected", "Unauthorized http response (status code 401) was expected");

        //    // Return the authority URL.
        //    // _Resource = parameters.Resource;

        //}

        //public async Task<IAuthenticationTokenResult> ExecuteAuthenticationRequest(Func<AuthenticationContext, Task<AuthenticationResult>> authenticate)
        //{
        //    // Obtain an authentication token to access the web service.


        //    try
        //    {
        //        var authenticationContext = _authContext ?? await GetAuthenticationContext();
        //        var result = await authenticate(authenticationContext).ConfigureAwait(false);

        //        var authTokenResult = new AuthenticationTokenResult(true, result.AccessToken);
        //        _LastToken = authTokenResult;
        //        return authTokenResult;
        //    }
        //    catch (Exception e)
        //    {
        //        var exResult = new AuthenticationTokenResult(false, String.Empty);
        //        exResult.Exception = e;
        //        return exResult;
        //    }
        //}

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

        //public async Task<IAuthenticationTokenResult> GetAuthenticationTokenAsync(string username, string password)
        //{

        //    // Obtain an authentication token to access the web service.

        //    try
        //    {
        //        var authenticationContext = _authContext ?? await GetAuthenticationContext().ConfigureAwait(false);

        //        var clientAppDetails = GetClientDetails();
        //        var serverDetails = GetCrmServerDetails();
        //        var resource = serverDetails.CrmWebsiteUrl;

        //        var userCredential = new UserCredential(username, password);
        //        var result = await authenticationContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, userCredential);
        //        // return authContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, userCredential);

        //        //  var result = await authenticate(authenticationContext);
        //        var authTokenResult = new AuthenticationTokenResult(true, result.AccessToken);
        //        _LastToken = authTokenResult;
        //        return authTokenResult;
        //    }
        //    catch (Exception e)
        //    {
        //        var exResult = new AuthenticationTokenResult(false, String.Empty);
        //        exResult.Exception = e;
        //        return exResult;
        //    }


        //    //var result = await this.ExecuteAuthenticationRequest(authContext =>
        //    //    {
        //    //        var clientAppDetails = GetClientDetails();
        //    //        var serverDetails = GetCrmServerDetails();
        //    //        var resource = serverDetails.CrmWebsiteUrl;
        //    //        var userCredential = new UserCredential(username, password);
        //    //        return authContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, userCredential);
        //    //    });
        //    //  return result;
        //}

        //public async Task<IAuthenticationTokenResult> GetAuthenticateTokenAsync(IPlatformParameters platformParams)
        //{



        //    //var result = await this.ExecuteAuthenticationRequest(authContext =>
        //    //{
        //    //    var clientAppDetails = GetClientDetails();
        //    //    var serverDetails = GetCrmServerDetails();
        //    //    var resource = serverDetails.CrmWebsiteUrl;
        //    //    return authContext.AcquireTokenAsync(resource.ToString(), clientAppDetails.ClientId, clientAppDetails.RedirectUri, _clientAppInfo.PlatformParams);
        //    //});

        //    // return result;
        //}

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
