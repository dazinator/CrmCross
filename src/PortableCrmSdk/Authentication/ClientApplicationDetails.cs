using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{
    /// <summary>
    /// The details of the Client Application that wishes to be granted access to the Crm Organisation.
    /// </summary>
    public class ClientApplicationDetails
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clientId">The client id, configured in ADFS or Azure Management Portal.</param>
        /// <param name="redirectUri">The redirectUri, configured in ADFS or Azure Management Portal.</param>
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

}
