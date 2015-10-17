using System;

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
