using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CrmCross.Authentication
{

    /// <summary>
    /// The username and password details of the account used to authenticate.
    /// </summary>
    public class UsernamePasswordCredential
    {
        private string _password;

        public UsernamePasswordCredential(string username, string password)
        {
            _password = password;
            UserCredential = new UserCredential(username, password);
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
                UserCredential = new UserCredential(value, _password);
            }
        }

        /// <summary>
        /// The password.
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                UserCredential = new UserCredential(UserCredential.UserName, value);
                _password = value;
            }
        }

        /// <summary>
        /// The ADAL UserCredential.
        /// </summary>
        public UserCredential UserCredential { get; set; }

    }
}
