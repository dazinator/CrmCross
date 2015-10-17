using CrmCross.Authentication;
using CrmCross.Http;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CrmCross.Tests
{
    public abstract class AdalAuthenticationProviderTests
    {
        [TestCase(TestConfig.Crm_2013_Online_Org_Url)]
        public async Task Can_Get_Auth_Token(string crmWebsiteUrl)
        {
            var authDetails = GetAuthenticationDetailsProvider();
            authDetails.CrmServerDetails.CrmWebsiteUrl = new Uri(crmWebsiteUrl);
            authDetails.UserCredentials = null;

            var httpClientFactory = GetHttpClientFactory();

            var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(authDetails, httpClientFactory);
            var tokenResult = await tokenProvider.GetAuthenticateTokenAsync().ConfigureAwait(false);
            Assert.That(tokenResult, Is.Not.Null);
            Assert.That(tokenResult.AccessToken, Is.Not.Null);
        }    

        [TestCase(TestConfig.Crm_2013_Online_Org_Url, TestConfig.Username)]
        public async Task Can_Get_Auth_Token_Using_Username_And_Password(string crmWebsiteUrl, string userName)
        {

            var authDetails = GetAuthenticationDetailsProvider();
            authDetails.CrmServerDetails.CrmWebsiteUrl = new Uri(crmWebsiteUrl);          

            var fileSystem = GetFileSystem();
            var password = TestConfig.GetPassword(fileSystem);

            authDetails.UserCredentials = new UsernamePasswordCredential(userName, password);

            var httpClientFactory = GetHttpClientFactory();
            var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(authDetails, httpClientFactory);

            var tokenResult = await tokenProvider.GetAuthenticateTokenAsync().ConfigureAwait(false); 
            Assert.That(tokenResult, Is.Not.Null);
            Assert.That(tokenResult.Success);
            Assert.That(tokenResult.AccessToken, Is.Not.Null);
            Assert.That(tokenResult.AccessToken, Is.Not.EqualTo(string.Empty));
            System.Diagnostics.Debug.WriteLine(tokenResult.AccessToken);
            //  Console.WriteLine(tokenResult.AccessToken);

        }

        #region Platform Specific Dependencies
        protected abstract IAuthenticationDetailsProvider GetAuthenticationDetailsProvider();
        protected abstract IFileSystem GetFileSystem();
        protected abstract IHttpClientFactory GetHttpClientFactory();
        #endregion

    }




}
