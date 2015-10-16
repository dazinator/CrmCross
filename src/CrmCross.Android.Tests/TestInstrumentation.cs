using System;

using Android.App;
using Android.Runtime;
using Xamarin.Android.NUnitLite;
using System.Reflection;

namespace CrmCross.Android.Tests
{
    [Instrumentation(Name = "crmcross.android.tests.TestInstrumentation")]
    public class TestInstrumentation : TestSuiteInstrumentation
    {

        public TestInstrumentation(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        protected override void AddTests()
        {
            AddTest(Assembly.GetExecutingAssembly());          
        }
    }

}