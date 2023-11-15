public interface IJwtAuthenticationService
{
    string? AuthenticateByUsernameAndPassword(string username, string password);
}
