using System;

namespace CrmCross.Authentication
{
    /// <summary>
    /// Details of the Crm Server that we wish to be granted access to.
    /// </summary>
    public class CrmServerDetails
    {
        public const string DefaultOrganisationWebservicePath = "/XRMServices/2011/Organization.svc";
        public const string DefaultOrganisationRestEndpointPath = "/XRMServices/2011/OrganizationData.svc";
       

        private readonly static Version SdkClientVersion = new Version("7.0.0.0");

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="crmWebsiteUrl">The url of the Dynamics Crm website.</param>
        /// <param name="organisationWebServiceUrl">The url of the organisation web service (svc). If not specified (null) then it will be formulated based on the <paramref name="crmWebsiteUrl"/>Crm Website Url</param>
        /// <param name="authenticationEndpointUrl">The url of the authentication endpoint. If not specified (null) then it will be formulated based on the <paramref name="organisationWebServiceUrl"/>Organisation WebService Url</param>
        public CrmServerDetails(Uri crmWebsiteUrl, Uri organisationWebServiceUrl = null, Uri authenticationEndpointUrl = null)
        {
            if (crmWebsiteUrl == null)
            {
                throw new ArgumentNullException("crmWebsiteUrl");
            }
            CrmWebsiteUrl = crmWebsiteUrl;
            var slashChars = new char[] { '/', '\\' };
            if (organisationWebServiceUrl == null)
            {              
                organisationWebServiceUrl = new Uri(String.Format("{0}{1}", crmWebsiteUrl.ToString().TrimEnd(slashChars), DefaultOrganisationWebservicePath));
            }
            OrganisationWebServiceUrl = organisationWebServiceUrl;
            OrganisationWebEndpointUrl = new Uri(String.Format("{0}/web", OrganisationWebServiceUrl));

            if (authenticationEndpointUrl == null)
            {
                string authEndpointUrl = string.Format("{0}?SdkClientVersion={1}", OrganisationWebEndpointUrl, SdkClientVersion.ToString());
                authenticationEndpointUrl = new Uri(authEndpointUrl);
            }
            AuthenticationEndpointUrl = authenticationEndpointUrl;          
            OrganisationRestEndpointUrl = new Uri(String.Format("{0}{1}", crmWebsiteUrl.ToString().TrimEnd(slashChars), DefaultOrganisationRestEndpointPath));
           
        }

        /// <summary>
        /// The Uri of the Crm Website.
        /// </summary>
        public Uri CrmWebsiteUrl { get; set; }

        /// <summary>
        /// The Uri of the Crm Organisation Web Service.
        /// </summary>
        public Uri OrganisationWebServiceUrl { get; set; }

        /// <summary>
        /// The Uri of the Crm Organisation Rest Endpoint.
        /// </summary>
        public Uri OrganisationRestEndpointUrl { get; set; }

        /// <summary>
        /// The Uri of the Crm Organisation Rest Endpoint.
        /// </summary>
        public Uri OrganisationWebEndpointUrl { get; set; }


        /// <summary>
        /// The Dynamics CRM Authentication Endpoint URI.
        /// </summary>
        public Uri AuthenticationEndpointUrl { get; set; }

       

    }

}
