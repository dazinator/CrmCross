
using Android.App;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CrmCross.Authentication
{
    /// <summary>
    /// Android platform specific parameters.
    /// </summary>
    public class AndroidPlatform : Platform
    {
        public AndroidPlatform(Activity callingActivity)
        {
            PlatformParameters = new PlatformParameters(callingActivity);
        }
    }
  
}