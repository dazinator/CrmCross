using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CrmCross.Authentication
{
    public interface IPlatform
    {

    }

    public abstract class Platform : IPlatform
    {
        protected internal IPlatformParameters PlatformParameters { get; set; }
    }

}
