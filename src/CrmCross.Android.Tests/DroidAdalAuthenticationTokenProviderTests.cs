using System;
using NUnit.Framework;
using System.Threading.Tasks;
using CrmCross.Authentication;
using CrmCross.Tests;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Android.App;

namespace CrmCross.Android.Tests
{
    [TestFixture]
    public class DroidAdalAuthenticationTokenProviderTests : AdalAuthenticationProviderTests
    {

        [SetUp]
        public void Setup() { }

        [TearDown]
        public void Tear() { }   


        protected override IPlatformParameters GetPlatformParameters()
        {
          //  var activity = MainActivity.Instance;
            var activity = new TestActivity();

            var platFormParams = new PlatformParameters(activity);
            return platFormParams;
        }
    }
}