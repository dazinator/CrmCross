using CrmCross.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CrmCross.Tests
{
    public abstract class AdalAuthenticationProviderTests
    {

        [TestCase(TestConfig.Crm_2013_Online_Org_Url)]
        public async Task Can_Get_Auth_Token(string crmWebsiteUrl)
        {

            AdalClientApplicationDetails clientDetails = GetClientApplicationDetails();
            CrmServerDetails crmServerDetails = GetCrmServerDetails(crmWebsiteUrl);
            var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(clientDetails, crmServerDetails);
            var tokenResult = await tokenProvider.GetAuthenticateTokenAsync();
            Assert.That(tokenResult, Is.Not.Null);
            Assert.That(tokenResult.AccessToken, Is.Not.Null);

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

        protected abstract IPlatformParameters GetPlatformParameters();

        [TestCase(TestConfig.Crm_2013_Online_Org_Url, TestConfig.Username)]
        public async Task Can_Get_Auth_Token_Using_Username_And_Password(string crmWebsiteUrl, string userName)
        {

            AdalClientApplicationDetails clientDetails = GetClientApplicationDetails();
            CrmServerDetails crmServerDetails = GetCrmServerDetails(crmWebsiteUrl);

            var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(clientDetails, crmServerDetails);
            var password = TestConfig.GetPassword();

            var tokenResult = await tokenProvider.GetAuthenticationTokenAsync(userName, password);
            Assert.That(tokenResult, Is.Not.Null);
            Assert.That(tokenResult.AccessToken, Is.Not.Null);

            //  Console.WriteLine(tokenResult.AccessToken);

        }
    }



}
