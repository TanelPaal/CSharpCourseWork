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
    public readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Player>> _gameSessions = new ConcurrentDictionary<string, ConcurrentDictionary<string, Player>>();
    private const int MaxPlayersPerGame = 2;

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

        if (_gameSessions[gameId].ContainsKey(username))
        {
            // Player is rejoining, retain their piece
            return true;
        }
        
        if (CanAddPlayerToGame(gameId))
        {

            var playerCount = _gameSessions[gameId].Count;
            EGamePiece piece = playerCount == 0 ? EGamePiece.X : EGamePiece.O;

            // Ensure the piece is not already taken
            if (_gameSessions[gameId].Values.Any(p => p.Piece == ConvertToXO(piece)))
            {
                piece = piece == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
            }

            string gamePiece = ConvertToXO(piece);
            Console.WriteLine(gamePiece + " pieces have been assigned to "+ username);
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