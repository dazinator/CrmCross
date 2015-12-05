using System;
using System.Linq;

using Android.App;
using Android.Content;

namespace CrmCross.Authentication
{
    /// <summary>
    /// Provides authentication details necessary for authentication token aquisition on Android.
    /// </summary>
    public class AndroidAuthenticationDetailsProvider : AuthenticationDetailsProvider
    {
        private Activity _activity;       

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clientId">The client id of our application - identifies the application wishing to be granted access to CRM.</param>
        /// <param name="crmWebsiteServerUrl">The url of the Dynamics Crm website that we wish to be granted access to.</param>
        /// <param name="callingActivity">The current android activity. This is inspected for DataHost and DataScheme attribute information (IntentFilter) which is used to formulate a return uri, which allows a sign in page to navigate back to the activity after sign in has been completed. This is only used if you do not explicitly provide a returnUrl argument.</param>
        /// <param name="returnUrl">The return URI that configured for our client application. This URI is the one used by the sign in page when it needs to navigate back to the activity.</param>
        public AndroidAuthenticationDetailsProvider(string clientId, Uri crmWebsiteServerUrl, Activity callingActivity, string returnUrl = "")
        {
            _activity = callingActivity;
            Uri returnUri = null;

            if (_activity != null && string.IsNullOrEmpty(returnUrl))
            {
                returnUri = DiscoverActivityReturnUri(callingActivity);
            }

            ClientApplicationDetails = new ClientApplicationDetails(clientId, returnUri);
            CrmServerDetails = new CrmServerDetails(crmWebsiteServerUrl);
            UserCredentials = null; // new UsernamePasswordCredential(String.Empty, String.Empty);
        }

        /// <summary>
        ///  Inspects the attributes present on the Activity class, to get the DataScheme and DataHost which are necessary to indicate the return Url allowing a sign in page to navigate back to the activity after sign in.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static Uri DiscoverActivityReturnUri(Activity activity)
        {
            var intentFilterAtts = activity.GetType().GetCustomAttributes(typeof(IntentFilterAttribute), true);
            foreach (IntentFilterAttribute intentFilter in intentFilterAtts)
            {
                // look for ActionView, with CategoryDefault and CategoryBrowsable
                //   [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "appschema", DataHost = "crmcross.droid")]
                if (intentFilter.Actions != null && intentFilter.Actions.Any(a => a == Intent.ActionView))
                {
                    if (intentFilter.Categories != null && intentFilter.Categories.Any(a => a == Intent.CategoryDefault))
                    {
                        if (intentFilter.Categories != null && intentFilter.Categories.Any(a => a == Intent.CategoryBrowsable))
                        {
                            Uri returnUri = new Uri(string.Format("{0}://{1}", intentFilter.DataScheme, intentFilter.DataHost));
                            return returnUri;
                        }
                    }
                }
            }

            return null;
        }

        public override ClientApplicationDetails ClientApplicationDetails { get; protected set; }

        public override CrmServerDetails CrmServerDetails { get; protected set; }

        public override UsernamePasswordCredential UserCredentials { get; set; }

        /// <summary>
        /// Returns the platform.
        /// </summary>
        public override Platform Platform
        {
            get
            {
                if (_activity == null)
                {
                    return null;
                }
                return new AndroidPlatform(_activity);
            }
        }

    }
}