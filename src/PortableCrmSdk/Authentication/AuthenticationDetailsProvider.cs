using CrmCross.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{

    public abstract class AuthenticationDetailsProvider : IAuthenticationDetailsProvider
    {

        public abstract UsernamePasswordCredential UserCredentials { get; set; }
        public abstract Platform Platform { get; }
        public abstract CrmServerDetails CrmServerDetails { get; protected set; }
        public abstract ClientApplicationDetails ClientApplicationDetails { get; protected set; }

        //public abstract ClientApplicationDetails GetClientApplicationDetails();
        //public abstract CrmServerDetails GetCrmServerDetails();
        // public abstract Platform GetPlatform();
        //  public abstract UsernamePasswordCredential GetUserCredentials();




      
    }
}
