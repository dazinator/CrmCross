using CrmCross.Authentication;
using CrmCross.Http;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CrmCross.Tests
{
    public abstract class AdalAuthenticationProviderTests
    {      

        [TestCase(TestConfig.Crm_2013_Online_Org_Url)]
        public void Can_Get_Auth_Token(string crmWebsiteUrl)
        {

            var y = new AutoResetEvent(false);

            this.RunOnMainThread(() =>
            {
                var authDetails = GetAuthenticationDetailsProvider();
                authDetails.CrmServerDetails.CrmWebsiteUrl = new Uri(crmWebsiteUrl);
                authDetails.UserCredentials = null;

                var httpClientFactory = GetHttpClientFactory();

                var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(authDetails, httpClientFactory);
                IAuthenticationTokenResult result = null;

                Task.Run(() => tokenProvider.GetAuthenticateTokenAsync())
                    .ContinueWith((a) =>
                    {
                        result = a.Result;
                    })
                .Wait();

                y.Set();
            });

            y.WaitOne(new TimeSpan(0,1,0));

        }    

        [TestCase(TestConfig.Crm_2013_Online_Org_Url, TestConfig.Username)]
        public void Can_Get_Auth_Token_Using_Username_And_Password(string crmWebsiteUrl, string userName)
        {

            var y = new AutoResetEvent(false);
            IAuthenticationTokenResult result = null;

            this.RunOnMainThread(() =>
            {
                var authDetails = GetAuthenticationDetailsProvider();
                authDetails.CrmServerDetails.CrmWebsiteUrl = new Uri(crmWebsiteUrl);

                var fileSystem = GetFileSystem();
                var password = TestConfig.GetPassword(fileSystem);

                authDetails.UserCredentials = new UsernamePasswordCredential(userName, password);

                var httpClientFactory = GetHttpClientFactory();
                var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(authDetails, httpClientFactory);
               

                Task.Run(() => tokenProvider.GetAuthenticateTokenAsync())
                    .ContinueWith((a) => {
                        result = a.Result;
                    })
                .Wait();
                y.Set();                          

            });

            y.WaitOne(new TimeSpan(0, 1, 0));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success);
            Assert.That(result.AccessToken, Is.Not.Null);
            Assert.That(result.AccessToken, Is.Not.EqualTo(string.Empty));
            System.Diagnostics.Debug.WriteLine(result.AccessToken);                   

        }
              

        #region Platform Specific Dependencies
        protected abstract IAuthenticationDetailsProvider GetAuthenticationDetailsProvider();
        protected abstract IFileSystem GetFileSystem();
        protected abstract IHttpClientFactory GetHttpClientFactory();
        protected abstract void RunOnMainThread(Action action);
        #endregion

    }




}
