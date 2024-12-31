using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebApp.GameServ;

namespace WebApp.Hubs
{
    public class GameHub : Hub
    {
        public readonly GameService _gameService;

        public GameHub(GameService gameService)
        {
            _gameService = gameService;
        }

        private Dictionary<string, string> ConnectionUsernames = new();

        public override async Task OnConnectedAsync()
        {
            await Task.Delay(500);

            var httpContext = Context.GetHttpContext();
            var gameId = httpContext?.Request.Query["gameId"];
            var username = httpContext?.Request.Query["username"];

            if (!string.IsNullOrEmpty(gameId) && !string.IsNullOrEmpty(username))
            {
                // Check if the player already exists in the game
                var existingPlayer = _gameService.IsPlayerInGame(gameId, username);

                if (existingPlayer || _gameService.AddPlayerToGame(gameId, username))
                {
                    ConnectionUsernames[Context.ConnectionId] = username;
                    await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                    Console.WriteLine(
                        $"Connection {Context.ConnectionId} for user {username} added/reconnected to group {gameId}");
                    var sessions = _gameService._gameSessions;
                    await Clients.Caller.SendAsync("ReceiveGameSessionData", sessions);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Unable to join the game. The game might be full or an error occurred.");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Task.Delay(500);

            var httpContext = Context.GetHttpContext();
            var gameId = httpContext?.Request.Query["gameId"];
            var username = httpContext?.Request.Query["username"];

            // Don't remove the player immediately
            // Instead, wait for a short period to see if they reconnect
            if (!string.IsNullOrEmpty(gameId))
            {
                // Store the disconnection time
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
                Console.WriteLine($"Connection {Context.ConnectionId} temporarily disconnected from group {gameId}");
            }

            ConnectionUsernames.Remove(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}