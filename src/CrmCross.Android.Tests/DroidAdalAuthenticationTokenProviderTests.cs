using System;
using NUnit.Framework;
using CrmCross.Authentication;
using CrmCross.Tests;
using CrmCross.Http;
using Android.OS;
using System.Threading;

namespace CrmCross.Android.Tests
{
    [TestFixture]
    public class DroidAdalAuthenticationTokenProviderTests : AdalAuthenticationProviderTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void Tear() { }

        protected override IAuthenticationDetailsProvider GetAuthenticationDetailsProvider()
        {
            var activity = new TestActivity();
            string orgUrl = TestConfig.Crm_2013_Online_Org_Url;
            Uri crmWebsiteUrl = new Uri(orgUrl);
            AndroidAuthenticationDetailsProvider authDetailsProvider = new AndroidAuthenticationDetailsProvider(TestConfig.NativeClientId, crmWebsiteUrl, activity);
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

        protected override void RunOnMainThread(Action action)
        {
            // we are allready on main thread.
            if (SynchronizationContext.Current != null)
            {
                action();
                return;
            }

            // post to main thread.
            using (var h = new Handler(Looper.MainLooper))
            {
                h.Post(action);
            }
        }
    }
}