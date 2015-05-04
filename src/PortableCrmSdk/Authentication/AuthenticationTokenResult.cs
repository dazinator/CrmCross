using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{
    public class AuthenticationTokenResult : IAuthenticationTokenResult
    {
        private bool _success;
        private AuthenticationResult _authResult;

        internal AuthenticationTokenResult(AuthenticationResult authResult)
        {
            _success = authResult != null;

        }

        public bool Success
        {
            get { return _success; }
        }

        public string AccessToken
        {
            get { return _authResult == null ? string.Empty : _authResult.AccessToken; }
        }

        public string AccessTokenType
        {
            get { return _authResult == null ? string.Empty : _authResult.AccessTokenType; }
        }

        public Exception Exception { get; set; }
    }
}
