using System;

using Android.App;
using Android.Runtime;
using Xamarin.Android.NUnitLite;
using System.Reflection;
using Android.OS;
using Java.Lang;

namespace CrmCross.Android.Tests
{
    [Instrumentation(Name = "crmcross.android.tests.TestInstrumentation")]
    public class TestInstrumentation : TestyDroid.Android.TestyDroidTestSuiteInstrumentation
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