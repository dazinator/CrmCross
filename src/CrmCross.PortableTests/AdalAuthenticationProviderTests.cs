using CrmCross.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(authDetails);
            var tokenResult = await tokenProvider.GetAuthenticateTokenAsync();
            Assert.That(tokenResult, Is.Not.Null);
            Assert.That(tokenResult.AccessToken, Is.Not.Null);
        }


        protected abstract IAuthenticationDetailsProvider GetAuthenticationDetailsProvider();

        [TestCase(TestConfig.Crm_2013_Online_Org_Url, TestConfig.Username)]
        public async Task Can_Get_Auth_Token_Using_Username_And_Password(string crmWebsiteUrl, string userName)
        {

            var authDetails = GetAuthenticationDetailsProvider();
            authDetails.CrmServerDetails.CrmWebsiteUrl = new Uri(crmWebsiteUrl);
            authDetails.UserCredentials.Username = userName;
            authDetails.UserCredentials.Password = TestConfig.GetPassword();

            var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(authDetails);

            var tokenResult = await tokenProvider.GetAuthenticateTokenAsync();
            Assert.That(tokenResult, Is.Not.Null);
            Assert.That(tokenResult.AccessToken, Is.Not.Null);

            //  Console.WriteLine(tokenResult.AccessToken);

        }




    }




}
