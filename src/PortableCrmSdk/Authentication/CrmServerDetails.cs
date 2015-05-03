using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Authentication
{
    public class CrmServerDetails
    {
        public const string DefaultOrganisationWebservicePath = "/XRMServices/2011/Organization.svc";
        private readonly static Version SdkClientVersion = new Version("7.0.0.0");

        public CrmServerDetails(Uri crmWebsiteUrl, Uri organisationWebServiceUrl = null, Uri authenticationEndpointUrl = null)
        {
            if (crmWebsiteUrl == null)
            {
                throw new ArgumentNullException("crmWebsiteUrl");
            }
            CrmWebsiteUrl = crmWebsiteUrl;
            if (organisationWebServiceUrl == null)
            {
                var slashChars = new char[] { '/', '\\' };
                organisationWebServiceUrl = new Uri(String.Format("{0}{1}", crmWebsiteUrl.ToString().TrimEnd(slashChars), DefaultOrganisationWebservicePath));
            }
            OrganisationWebServiceUrl = organisationWebServiceUrl;
            if (authenticationEndpointUrl == null)
            {
                string authEndpointUrl = string.Format("{0}/web?SdkClientVersion={1}", OrganisationWebServiceUrl, SdkClientVersion.ToString());
                authenticationEndpointUrl = new Uri(authEndpointUrl);
            }
            AuthenticationEndpointUrl = authenticationEndpointUrl;
        }

        public Uri CrmWebsiteUrl { get; set; }
        public Uri OrganisationWebServiceUrl { get; set; }
        public Uri AuthenticationEndpointUrl { get; set; }
    }

}
