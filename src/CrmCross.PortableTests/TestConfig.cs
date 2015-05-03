using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Tests
{
    public static class TestConfig
    {
        /// <summary>
        /// This is a ClientID that was given once I registered a "Web" application via the Azure Management Portal.
        /// </summary>
        public const string WebClientID = "596cdb85-c417-4e44-a9b3-ce7b0706d4ae";

        /// <summary>
        /// This is a ClientID that was given once I registered a "Native" application via the Azure Management Portal.
        /// </summary>
        public const string NativeClientId = "0f15c97a-217d-4a5a-832d-5bd16e308281";

        /// <summary>
        /// This is the Redirect Url that I entered when registering the application through the Azure Management Portal.
        /// </summary>
        public const string NativeClientRedirectUrl = "http://someurl.com";

        /// <summary>
        /// This is the Url of the Dynamics CRM website.
        /// </summary>
        public const string Crm_2013_Online_Org_Url = "https://crmadotrial4.api.crm4.dynamics.com";

        /// <summary>
        /// I am delibrately storing the password for the user account outside of source control. Change this to the path to a file that contains the password.
        /// </summary>
        public const string CrmPasswordFile = "G:\\Temp\\crmpassword.txt";

        /// <summary>
        /// This is the Username of the User that will be authenticating with Dynamics CRM (we will be obtaining an access token for).
        /// </summary>
        public const string Username = "testing@crmadotrial4.onmicrosoft.com";

        /// <summary>
        /// This loads the password for the user that we will use when authenticating the user with Dynamics CRM to obtain the access token.
        /// </summary>
        /// <Remarks>I am delibrately storing the password outside of source control so that I do not reveal it.</Remarks>
        /// <returns></returns>
        public static string GetPassword()
        {
            return "integr@tion2";
            //  var password = File.ReadAllText(CrmPasswordFile);
            //   var password = ConfigurationManager.AppSettings["Password"];
            //   return password;
        }


    }
}
