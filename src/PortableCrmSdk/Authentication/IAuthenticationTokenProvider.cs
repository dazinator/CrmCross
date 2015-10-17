using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{
    public interface IAuthenticationTokenProvider
    {     

        /// <summary>
        /// Gets an authentication token, will not display any login prompt - so if no token avaialable, will error.
        /// </summary>
        /// <returns></returns>
        Task<IAuthenticationTokenResult> GetAuthenticateTokenAsync();      

        /// <summary>
        /// Returns the last successful token that was used.
        /// </summary>
        /// <returns></returns>
        IAuthenticationTokenResult GetLastToken();
    }
}
