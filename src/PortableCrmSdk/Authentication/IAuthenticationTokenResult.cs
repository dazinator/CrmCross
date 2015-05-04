using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{
    public interface IAuthenticationTokenResult
    {
        bool Success { get; }
        string AccessToken { get; }
        string AccessTokenType { get; }
        Exception Exception { get; }
    }
}
