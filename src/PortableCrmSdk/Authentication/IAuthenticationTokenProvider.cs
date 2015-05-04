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
        ///// <summary>
        ///// Get an authentication token, using the specified username and password. This will not display any prompts for login if authentication fails.
        ///// </summary>
        ///// <param name="username"></param>
        ///// <param name="password"></param>
        ///// <returns></returns>
        //Task<IAuthenticationTokenResult> GetAuthenticationTokenAsync(string username, string password);

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
