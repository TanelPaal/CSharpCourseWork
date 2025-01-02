using System.Collections.Concurrent;
using GameBrain;

namespace WebApp.GameServ;

public class Player
{
    public string Username { get; set; } = "";
    public string Piece { get; set; } = "";
}

public class GameService
{
    public ConcurrentDictionary<string, ConcurrentDictionary<string, Player>> _gameSessions = new ConcurrentDictionary<string, ConcurrentDictionary<string, Player>>();
    private const int MaxPlayersPerGame = 2;

    
    public async Task EndGame(string gameId)
    {
        if (_gameSessions.ContainsKey(gameId))
        {
            // Clear all players from the game
            _gameSessions[gameId].Clear();
            
            // Remove the game session entirely
            _gameSessions.TryRemove(gameId, out _);
            
            Console.WriteLine($"Game {gameId} has been ended and removed from active sessions");
        }
    }
    public bool CanAddPlayerToGame(string gameId)
    {
        if (!_gameSessions.ContainsKey(gameId))
        {
            _gameSessions[gameId] = new ConcurrentDictionary<string, Player>();
        }
        return _gameSessions[gameId].Count < MaxPlayersPerGame;
    }

    public bool AddPlayerToGame(string gameId, string username)
    {
        if (!_gameSessions.ContainsKey(gameId))
        {
            _gameSessions[gameId] = new ConcurrentDictionary<string, Player>();
        }

        // If player is already in the game, just return true
        if (_gameSessions[gameId].ContainsKey(username))
        {
            return true;
        }
        
        if (CanAddPlayerToGame(gameId))
        {
            var existingPieces = _gameSessions[gameId].Values
                .Select(p => p.Piece)
                .ToList();

            string gamePiece;
            if (!existingPieces.Contains("X"))
            {
                gamePiece = "X";
            }
            else if (!existingPieces.Contains("O"))
            {
                gamePiece = "O";
            }
            else
            {
                return false;
            }

            _gameSessions[gameId][username] = new Player { Username = username, Piece = gamePiece };
            return true;
        }
        return false;
    }

    public Player GetPlayer(string gameId, string username)
    {
        if (_gameSessions.ContainsKey(gameId) && _gameSessions[gameId].ContainsKey(username))
        {
            return _gameSessions[gameId][username];
        }
        throw new InvalidOperationException("Player not found in the game.");
    }

    public void RemovePlayerFromGame(string gameId, string username)
    {
        if (_gameSessions.ContainsKey(gameId))
        {
            var test = _gameSessions[gameId].Remove(username, out _);
            Console.WriteLine("we have "+ test + " to remove the player from the game");
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

    static string ConvertToXO(EGamePiece number)
    {
        return number switch
        {
            EGamePiece.X=> "X",
            EGamePiece.O => "O",
            _ => throw new ArgumentException("Input must be 1 or 0.")
        };
    }
}