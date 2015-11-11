using Android.OS;
using CrmCross.Android.Tests;
using CrmCross.Authentication;
using CrmCross.Http;
using NUnit.Framework;
using System;
using System.Threading;

namespace CrmCross.Tests
{
    [TestFixture]
    public class DroidOrganizationDataWebServiceProxyTests : OrganizationDataWebServiceProxyTests
    {

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
