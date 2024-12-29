using System.Collections.Concurrent;

namespace WebApp.UserServ;

using System.Collections.Concurrent;

public class UserService
{
    private readonly ConcurrentDictionary<string, bool> _loggedInUsers = new ConcurrentDictionary<string, bool>();
    private const int MaxPlayers = 10;

    public bool IsUsernameTaken(string username)
    {
        return _loggedInUsers.ContainsKey(username);
    }
    
    public bool CanAddUser()
    {
        return _loggedInUsers.Count < MaxPlayers;
    }

    public bool AddUser(string username)
    {
        if (CanAddUser())
        {
            return _loggedInUsers.TryAdd(username, true);
        }
        return false;
    }

    public void RemoveUser(string username)
    {
        _loggedInUsers.TryRemove(username, out _);
    }
}