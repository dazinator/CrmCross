using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CrmCross.Authentication;

namespace PortableCrmSdk.Authentication
{
    public class AndroidAuthenticationDetailsProvider : AuthenticationDetailsProvider
    {
        private Activity _activity;       

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