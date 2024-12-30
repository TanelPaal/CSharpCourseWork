using System.Collections.Concurrent;

namespace WebApp.GameServ;

public class GameService
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> _gameSessions = new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>();
    private const int MaxPlayersPerGame = 2;

    public bool CanAddPlayerToGame(string gameId)
    {
        if (!_gameSessions.ContainsKey(gameId))
        {
            _gameSessions[gameId] = new ConcurrentDictionary<string, bool>();
        }
        Console.WriteLine(_gameSessions[gameId].Count < MaxPlayersPerGame);
        return _gameSessions[gameId].Count < MaxPlayersPerGame;
    }

    public bool AddPlayerToGame(string gameId, string username)
    {
        Console.WriteLine(gameId + " " + username);
        if (CanAddPlayerToGame(gameId))
        {
            Console.WriteLine(username + " Added to game");
            return _gameSessions[gameId].TryAdd(username, true);
        }
        return false;
    }

    public void RemovePlayerFromGame(string gameId, string username)
    {
        if (_gameSessions.ContainsKey(gameId))
        {
            _gameSessions[gameId].TryRemove(username, out _);
        }
    }

    public bool IsPlayerInGame(string gameId, string username)
    {
        return _gameSessions.ContainsKey(gameId) && _gameSessions[gameId].ContainsKey(username);
    }

    public int GetPlayerCount(string gameId)
    {
        if (_gameSessions.ContainsKey(gameId))
        {
            Console.WriteLine(gameId + "'s playercount is " + _gameSessions[gameId].Count);
            return _gameSessions[gameId].Count;
        }
        return 0;
    }

    public string GetCurrentTurn(string gameId)
    {
        // Implement logic to get the current turn for the game
        return string.Empty;
    }

    public void SetNextTurn(string gameId, string username)
    {
        // Implement logic to set the next turn for the game
    }
}