public interface IUserService
{
    User? FindUserByUsernameAndPassword(string username, string password);
}
