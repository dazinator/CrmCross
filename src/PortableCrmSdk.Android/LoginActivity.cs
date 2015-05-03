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
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using CrmCross.Tests;

namespace PortableCrmSdk.Android
{
    [Activity(Label = "Login", MainLauncher=true)]
    public class LoginActivity : Activity
    {

        private string _CrmWebsiteUrl = TestConfig.Crm_2013_Online_Org_Url;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.Login);
            var accessTokenText = this.FindViewById<TextView>(Resource.Id.accessToken);
            var button = this.FindViewById<Button>(Resource.Id.loginButton);
            button.Click += async (s, e) =>
                {
                    AdalClientApplicationDetails clientDetails = GetClientApplicationDetails();
                    CrmServerDetails crmServerDetails = GetCrmServerDetails(_CrmWebsiteUrl);
                    var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(clientDetails, crmServerDetails);
                    var tokenResult = await tokenProvider.GetAuthenticateTokenAsync();
                    accessTokenText.Text = tokenResult.AccessToken;
                };

        }

        private CrmServerDetails GetCrmServerDetails(string crmUrl)
        {
            var crmUri = new Uri(crmUrl);
            CrmServerDetails crmServerDetails = new CrmServerDetails(crmUri);
            return crmServerDetails;
        }

        private AdalClientApplicationDetails GetClientApplicationDetails()
        {
            var redirectUrl = new Uri(TestConfig.NativeClientRedirectUrl);
            var platformParams = GetPlatformParameters();
            var clientDetails = new AdalClientApplicationDetails(TestConfig.NativeClientId, redirectUrl, platformParams);
            return clientDetails;
        }

        protected virtual IPlatformParameters GetPlatformParameters()
        {
            var platFormParams = new PlatformParameters(this);
            return platFormParams;
        }
    }
}