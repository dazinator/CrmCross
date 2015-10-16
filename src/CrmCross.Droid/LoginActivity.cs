using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using CrmCross.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using CrmCross.Tests;
using System.Threading.Tasks;
using PortableCrmSdk.Authentication;
using PortableCrmSdk.Http;

namespace CrmCross.Droid
{
    // <intent-filter>          
    //<action android:name="android.intent.action.VIEW"/>     
    //<category android:name="android.intent.category.DEFAULT"/>
    //<category android:name="android.intent.category.BROWSABLE"/>
    //<data android:scheme="appSchema" android:host="appName"/> 
    //</intent-filter>
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "appschema", DataHost = "crmcross.droid")]
    [Activity(Label = "CrmCross.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
             

        private IAuthenticationTokenProvider _authTokenProvider = null;
        private IAuthenticationDetailsProvider _authDetailsProvider = null;       

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            _authDetailsProvider = GetAuthenticationDetailsProvider();
            _authTokenProvider = GetAuthenticationTokenProvider(_authDetailsProvider);

            var button = this.FindViewById<Button>(Resource.Id.loginButton);
            button.Click += loginButton_click;

            var secondButton = this.FindViewById<Button>(Resource.Id.loginUsernamePasswordButton);
            secondButton.Click += secondButton_Click;
        }

        private AdalAuthenticationTokenProvider GetAuthenticationTokenProvider(IAuthenticationDetailsProvider authenticationDetailsProvider)
        {
            return new AdalAuthenticationTokenProvider(authenticationDetailsProvider, new AndroidHttpClientFactory());
        }

        private AndroidAuthenticationDetailsProvider GetAuthenticationDetailsProvider()
        {
            Uri crmWebsiteUrl = new Uri(TestConfig.Crm_2013_Online_Org_Url);
            AndroidAuthenticationDetailsProvider authDetailsProvider = new AndroidAuthenticationDetailsProvider(TestConfig.NativeClientId, crmWebsiteUrl, this);
            return authDetailsProvider;
        }

        async void secondButton_Click(object sender, EventArgs e)
        {
            var fileSystem = new FileSystem();
            await LogInUser(TestConfig.Username, TestConfig.GetPassword(fileSystem));
        }

        async void loginButton_click(object sender, EventArgs e)
        {
            await LogInUser();
        }

        private async Task LogInUser(string username, string password)
        {
            // Before we attempt token aquisition, set a username and password.
            _authDetailsProvider.UserCredentials.Username = username;
            _authDetailsProvider.UserCredentials.Password = password;

            var tokenResult = await _authTokenProvider.GetAuthenticateTokenAsync();
            var accessTokenText = this.FindViewById<TextView>(Resource.Id.accessToken);
            accessTokenText.Text = tokenResult.AccessToken;
        }

        private async Task LogInUser()
        {
            var tokenResult = await _authTokenProvider.GetAuthenticateTokenAsync();
            var accessTokenText = this.FindViewById<TextView>(Resource.Id.accessToken);
            accessTokenText.Text = tokenResult.AccessToken;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

    }
}