using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebApp.GameServ;

namespace WebApp.Hubs
{
    public class GameHub : Hub
    {
        private readonly GameService _gameService;

        public GameHub(GameService gameService)
        {
            _gameService = gameService;
        }

        public async Task JoinGame(string gameId, string username)
        {
            Console.WriteLine(username + "has joined");

            if (string.IsNullOrEmpty(gameId))
            {
                throw new ArgumentNullException(nameof(gameId), "Game ID cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");
            }

            if (_gameService.AddPlayerToGame(gameId, username))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                await Clients.Group(gameId).SendAsync("UpdateTurn", _gameService.GetCurrentTurn(gameId));
            }
            else
            {
                throw new InvalidOperationException("Unable to join the game. The game might be full or an error occurred.");
            }
        }

        public async Task LeaveGame(string gameId, string username)
        {
            _gameService.RemovePlayerFromGame(gameId, username);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }
    }
}