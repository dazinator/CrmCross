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
        private string _accessToken;

        public AuthenticationTokenResult(bool success, string accessToken)
        {
            _success = success;
            _accessToken = accessToken;
        }

        public bool Success
        {
            get { return _success; }
        }

        public string AccessToken
        {
            get { return _accessToken; }
        }

        public Exception Exception { get; set; }
    }
}
