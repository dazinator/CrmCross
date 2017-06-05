using CrmCross.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{
    /// <summary>
    /// Provides authentication details necessary for authentication token aquisition.
    /// </summary>
    public abstract class AuthenticationDetailsProvider : IAuthenticationDetailsProvider
    {
        /// <summary>
        /// The username / password details for the user wishing to authenticate.
        /// </summary>
        public abstract UsernamePasswordCredential UserCredentials { get; set; }
        /// <summary>
        /// Platform specific object, set appropriately per platform.
        /// </summary>
        public abstract Platform Platform { get; }

        /// <summary>
        /// The details of the CRM server / organisation we wish to access.
        /// </summary>
        public abstract CrmServerDetails CrmServerDetails { get; protected set; }

        /// <summary>
        /// The details of our client application wishing to be granted access.
        /// </summary>
        public abstract ClientApplicationDetails ClientApplicationDetails { get; protected set; }   
    }
}
