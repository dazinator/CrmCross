using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CrmCross.Authentication
{
    public class UsernamePasswordCredential
    {
        private string _password;

        public UsernamePasswordCredential(string username, string password)
        {
            _password = password;
            UserCredential = new UserCredential(username, password);
        }

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

        public UserCredential UserCredential { get; set; }

    }
}
