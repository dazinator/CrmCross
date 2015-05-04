using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CrmCross.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace PortableCrmSdk.Authentication
{
    public class AndroidPlatform : Platform
    {
        public AndroidPlatform(Activity callingActivity)
        {
            PlatformParameters = new PlatformParameters(callingActivity);
        }

    }

  
}