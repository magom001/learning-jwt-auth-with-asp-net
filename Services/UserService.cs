class UserService : IUserService
{
    private readonly IDictionary<(string, string), User> knownUsers = new Dictionary<
        (string, string),
        User
    >()
    {
        { ("admin", "pa$$word"), new User("admin", new[] { "admin" }) },
        { ("user", "pa$$word"), new User("user") }
    };

    public User? FindUserByUsernameAndPassword(string username, string password)
    {
        this.knownUsers.TryGetValue((username, password), out var user);

        return user;
    }
}
