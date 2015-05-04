using System;
using NUnit.Framework;
using System.Threading.Tasks;
using CrmCross.Authentication;
using CrmCross.Tests;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Android.App;
using PortableCrmSdk.Authentication;
using Android.Test;
using Android.OS;
using Android.Content;

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
            //var context = new global::Android.Content.Context()
            //var context = global::Android.Content.Context.ApplicationContext;

            //var activity2 = new Intent(context, typeof(TestActivity));
            ////  activity2.PutExtra ("MyData", "Data from Activity1");
            ////StartActivity (activity2);


            ////Intent intent = new Intent()
            //global::Android.Content.Context.StartActivity(activity2, Bundle.Empty);

            //   var instrumentation = new Instrumentation();         
            //  instrumentation.StartActivity()
            // instrumentation.CallActivityOnCreate(_activity, Bundle.Empty);

            //_activity.OnCreate(Bundle.Empty, PersistableBundle.Empty);
            // Instrumentation.
            Uri crmWebsiteUrl = new Uri(TestConfig.Crm_2013_Online_Org_Url);
            AndroidAuthenticationDetailsProvider authDetailsProvider = new AndroidAuthenticationDetailsProvider(TestConfig.NativeClientId, crmWebsiteUrl, _activity);
            return authDetailsProvider;
        }


    }
}