using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;

namespace CrmCross.Authentication
{
    /// <summary>
    /// The result of a Authentication Token aquisition request.
    /// </summary>
    public class AuthenticationTokenResult : IAuthenticationTokenResult
    {
        private bool _success;
        private AuthenticationResult _authResult;

        internal AuthenticationTokenResult(AuthenticationResult authResult)
        {
            _authResult = authResult;
            _success = authResult != null;
        }

        /// <summary>
        /// Whether token aquisition was successful.
        /// </summary>
        /// <remarks>If not successful, check Exception property for more details.</remarks>
        public bool Success
        {
            get { return _success; }
        }

        /// <summary>
        /// The token.
        /// </summary>
        public string AccessToken
        {
            get { return _authResult == null ? string.Empty : _authResult.AccessToken; }
        }

        /// <summary>
        /// The type of the Token.
        /// </summary>
        public string AccessTokenType
        {
            get { return _authResult == null ? string.Empty : _authResult.AccessTokenType; }
        }

        /// <summary>
        /// If Token aquisition was not successful, this will contain the exception.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
