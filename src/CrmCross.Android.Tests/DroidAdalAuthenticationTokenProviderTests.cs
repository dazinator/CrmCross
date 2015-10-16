using System;
using NUnit.Framework;
using CrmCross.Authentication;
using CrmCross.Tests;
using PortableCrmSdk.Authentication;
using CrmCross.Http;
using ModernHttpClient;
using System.Net.Http;
using PortableCrmSdk.Http;

namespace CrmCross.Android.Tests
{
    [TestFixture]
    public class DroidAdalAuthenticationTokenProviderTests : AdalAuthenticationProviderTests
    {

        private TestActivity _activity;

        [SetUp]
        public void Setup() { }

        [TearDown]
        public void Tear() { }
        
        protected override IAuthenticationDetailsProvider GetAuthenticationDetailsProvider()
        {
            _activity = new TestActivity();          

            string orgUrl = TestConfig.Crm_2013_Online_Org_Url;
            Uri crmWebsiteUrl = new Uri(orgUrl);

            AndroidAuthenticationDetailsProvider authDetailsProvider = new AndroidAuthenticationDetailsProvider(TestConfig.NativeClientId, crmWebsiteUrl, _activity);
            return authDetailsProvider;
        }

        protected override IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }

        protected override IHttpClientFactory GetHttpClientFactory()
        {
            return new AndroidHttpClientFactory();
        }
    }
}