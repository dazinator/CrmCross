using CrmCross.Authentication;
using CrmCross.Http;
using CrmCross.Messages;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CrmCross.Tests
{
    public abstract class OrganizationDataWebServiceProxyTests
    {

        [TestCase(TestConfig.Crm_2013_Online_Org_Url)]
        public void Can_Create_New_OrganizationDataWebServiceProxy(string crmWebsiteUrl)
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
                OrganizationDataWebServiceProxy sut = new OrganizationDataWebServiceProxy(authDetails.CrmServerDetails, tokenProvider, httpClientFactory);

                y.Set();
            });

            y.WaitOne(new TimeSpan(0, 1, 0));

        }

        [TestCase(TestConfig.Crm_2013_Online_Org_Url, TestConfig.Username)]
        public void Can_Authenticate(string crmWebsiteUrl, string userName)
        {

            var y = new AutoResetEvent(false);
            RetrieveVersionResponse response = null;

            this.RunOnMainThread(() =>
            {
                try
                {
                    var authDetails = GetAuthenticationDetailsProvider();
                    // authDetails.CrmServerDetails.CrmWebsiteUrl = new Uri(crmWebsiteUrl);

                    var fileSystem = GetFileSystem();
                    var password = TestConfig.GetPassword(fileSystem);

                    authDetails.UserCredentials = new UsernamePasswordCredential(userName);

                    var httpClientFactory = GetHttpClientFactory();
                    var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(authDetails, httpClientFactory);
                    OrganizationDataWebServiceProxy sut = new OrganizationDataWebServiceProxy(authDetails.CrmServerDetails, tokenProvider, httpClientFactory);

                    var request = new RetrieveVersionRequest();
                    response = (RetrieveVersionResponse)sut.Execute(request);

                    Assert.That(response != null);
                }
                catch (Exception e)
                {
                    //throw;
                }
                finally
                {

                    y.Set();
                }


            });

            y.WaitOne(new TimeSpan(0, 1, 0));

            Assert.That(response, Is.Not.Null);

            System.Diagnostics.Debug.WriteLine(response.Version);

        }


        #region Platform Specific Dependencies
        protected abstract IAuthenticationDetailsProvider GetAuthenticationDetailsProvider();
        protected abstract IFileSystem GetFileSystem();
        protected abstract IHttpClientFactory GetHttpClientFactory();
        protected abstract void RunOnMainThread(Action action);
        #endregion

    }




}
