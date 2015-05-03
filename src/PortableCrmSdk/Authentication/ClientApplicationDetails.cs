using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{
    public class ClientApplicationDetails
    {
        public ClientApplicationDetails(string clientId, Uri redirectUri)
        {
            if (String.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId");
            }
            if (redirectUri == null)
            {
                throw new ArgumentNullException("redirectUri");
            }
            ClientId = clientId;
            RedirectUri = redirectUri;
        }
        public string ClientId { get; set; }
        public Uri RedirectUri { get; set; }
    }

    public class AdalClientApplicationDetails : ClientApplicationDetails
    {
        public AdalClientApplicationDetails(string clientId, Uri redirectUri, IPlatformParameters platformParams)
            : base(clientId, redirectUri)
        {
            PlatformParams = platformParams;
        }
        public string ClientId { get; set; }
        public Uri RedirectUri { get; set; }
        public IPlatformParameters PlatformParams { get; set; }

    }

}
