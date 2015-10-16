namespace CrmCross.Authentication
{
    public interface IAuthenticationDetailsProvider
    {
        UsernamePasswordCredential UserCredentials { get; set; }
        Platform Platform { get; }
        CrmServerDetails CrmServerDetails { get; }
        ClientApplicationDetails ClientApplicationDetails { get; }
    }
}
