using System.Reflection;
using Android.App;
using Android.OS;
using Xamarin.Android.NUnitLite;

namespace CrmCross.Android.Tests
{
    [Activity(Label = "CrmCross.Android.Tests", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : TestSuiteActivity
    {

        public MainActivity()
        {
           // Instance = this;         
        }

      //  public static TestSuiteActivity Instance;

        protected override void OnCreate(Bundle bundle)
        {
            //Instance = this;

            // tests can be inside the main assembly
            AddTest(Assembly.GetExecutingAssembly());           

            // or in any reference assemblies
            // AddTest (typeof (Your.Library.TestClass).Assembly);          
            // Once you called base.OnCreate(), you cannot add more assemblies.
            base.OnCreate(bundle);
        }

        protected override void OnDestroy()
        {
           // Instance = null;
            base.OnDestroy();
        }
    }
}

