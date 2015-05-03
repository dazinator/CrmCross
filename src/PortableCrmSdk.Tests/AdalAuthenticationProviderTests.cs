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
    public class AdalAuthenticationProviderTests
    {

        [TestCase(Config.Crm_2013_Online_Org_Url)]
        public async Task Can_Get_Auth_Token(string crmWebsiteUrl)
        {

            AdalClientApplicationDetails clientDetails = GetClientApplicationDetails();
            CrmServerDetails crmServerDetails = GetCrmServerDetails(crmWebsiteUrl);

            var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(clientDetails, crmServerDetails);

            var tokenResult = await tokenProvider.GetAuthenticateTokenAsync();
            Assert.That(tokenResult, Is.Not.Null);
            Assert.That(tokenResult.AccessToken, Is.Not.Null);
            Console.WriteLine(tokenResult.AccessToken);
        }

        private CrmServerDetails GetCrmServerDetails(string crmUrl)
        {
            var crmUri = new Uri(crmUrl);
            CrmServerDetails crmServerDetails = new CrmServerDetails(crmUri);
            return crmServerDetails;
        }

        private AdalClientApplicationDetails GetClientApplicationDetails()
        {
            var redirectUrl = new Uri(Config.NativeClientRedirectUrl);
            var promptBehaviour = Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior.Auto;
            var platformParams = new PlatformParameters(promptBehaviour, this);
            var clientDetails = new AdalClientApplicationDetails(Config.NativeClientId, redirectUrl, platformParams);
            return clientDetails;
        }


        [TestCase(Config.Crm_2013_Online_Org_Url, Config.Username)]
        public async Task Can_Get_Auth_Token_Using_Username_And_Password(string crmWebsiteUrl, string userName)
        {

            AdalClientApplicationDetails clientDetails = GetClientApplicationDetails();
            CrmServerDetails crmServerDetails = GetCrmServerDetails(crmWebsiteUrl);

            var tokenProvider = (IAuthenticationTokenProvider)new AdalAuthenticationTokenProvider(clientDetails, crmServerDetails);
            var password = Config.GetPassword();

            var tokenResult = await tokenProvider.GetAuthenticationTokenAsync(userName, password);
            Assert.That(tokenResult, Is.Not.Null);
            Assert.That(tokenResult.AccessToken, Is.Not.Null);

            Console.WriteLine(tokenResult.AccessToken);

        }
    }



}
