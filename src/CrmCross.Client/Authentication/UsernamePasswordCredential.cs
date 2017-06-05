using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CrmCross.Authentication
{

    /// <summary>
    /// The username and password details of the account used to authenticate.
    /// </summary>
    public class UsernamePasswordCredential
    {
       // private string _password;

        public UsernamePasswordCredential(string username)
        {          
            UserCredential = new UserCredential(username);
        }

        /// <summary>
        /// The username.
        /// </summary>
        public string Username
        {
            get
            {
                return UserCredential.UserName;
            }
            set
            {
                UserCredential = new UserCredential(value);
            }
        }

        ///// <summary>
        ///// The password.
        ///// </summary>
        //public string Password
        //{
        //    get
        //    {
        //        return UserCredential.Password;
        //    }
        //    set
        //    {
        //        UserCredential = new UsernamePasswordCredential(UserCredential.Username, value);
        //        _password = value;
        //    }
        //}

        /// <summary>
        /// The ADAL UserCredential.
        /// </summary>
        public UserCredential UserCredential { get; set; }

    }
}
