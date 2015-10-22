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
    public class TestInstrumentation : TestSuiteInstrumentation
    {

        //public static TestActivity _testActivity;


        public TestInstrumentation(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        protected override void AddTests()
        {        
            
            AddTest(Assembly.GetExecutingAssembly());
        }
    }

    //public class LooperHandler : Handler
    //{
    //    public override void HandleMessage(Message msg)
    //    {
    //        base.HandleMessage(msg);
    //    }
    //}

    //public class LooperThread : Thread
    //{
    //    public Handler mHandler;

    //    public override void Run()
    //    {
    //        Looper.Prepare();
    //        mHandler = new LooperHandler();
    //        Looper.Loop();
    //    }

    //}

}