using ConsoleApp;
using DAL;
using GameBrain;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using WebApp.Hubs;
using WebApp.GameServ;

namespace WebApp.Pages.Game;

public class Play : PageModel
{
    private IGameRepository _gameRepository;
    private readonly IHubContext<GameHub> _hubContext;
    private TicTacTwoBrain? _gameBrain { get; set; }

    //now that we have the username client side we can have a table on the server with all the users data
    //we can then fetch that data from the server using @Model.data or w/e then use the username as the index
    //tomorrow access all the players data from the _gameService and put it into a table. then when the user refreshes
    //he can reference the data from the server.


    public string GameId = "";
    public Play(IGameRepository gameRepository, IHubContext<GameHub> hubContext, GameService gameService)
    {
        _gameRepository = gameRepository;
        _hubContext = hubContext;
        _gameService = gameService;


    }
    public GameService _gameService { get; set; }
    public GameState? GameState { get; set; }

    public ConcurrentDictionary<string, Player>? Players { get; set; }
    
    public bool IsMovableGrid(int x, int y)
    {
        // Implement your logic to determine if the cell (x, y) is part of the Movable Grid
        return IsInsidePlayableArea(x, y);
    }
    
    private bool IsInsidePlayableArea(int x, int y)
    {
        return _gameBrain!.IsInsidePlayableArea(x, y);
    }
    
    
    public async Task OnGetAsync(string gameId)
    {
        GameId = gameId;
        GameState = _gameRepository.GetSaveById(int.Parse(gameId));
        _gameBrain = new TicTacTwoBrain(GameState);
    }

    public async void OnPost(string gameId, int x, int y, int oldX, int oldY, string action)
    {
        GameId = gameId;
        GameState = _gameRepository.GetSaveById(int.Parse(gameId));
        _gameBrain = new TicTacTwoBrain(GameState);
        var tempGameBrain = _gameBrain;

        
        int[,] output = new int[4, 2];

        switch (action)
        {
            case "place":
                output[0, 0] = 1;
                output[1, 0] = x;
                output[1, 1] = y;
                GameController.ProcessInput((output, true, false), tempGameBrain);
                _gameBrain = tempGameBrain;
                _gameRepository.SaveGame(tempGameBrain._gameState, tempGameBrain._gameState.GameConfiguration.Name);
                await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate");
                break;

            case "moveGrid":
                output[0, 0] = 2;
                output[1, 0] = x;
                output[1, 1] = y;
                GameController.ProcessInput((output, true, false), tempGameBrain);
                _gameBrain = tempGameBrain;
                _gameRepository.SaveGame(tempGameBrain._gameState, tempGameBrain._gameState.GameConfiguration.Name);
                Console.WriteLine("sending gamestate update to " + GameId);
                await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate");
                break;

            case "movePiece":
                output[0, 0] = 3;
                output[1, 0] = x;
                output[1, 1] = y;
                output[2, 0] = oldX;
                output[2, 1] = oldY;
                GameController.ProcessInput((output, true, true), tempGameBrain);
                _gameBrain = tempGameBrain;
                _gameRepository.SaveGame(tempGameBrain._gameState, tempGameBrain._gameState.GameConfiguration.Name);
                await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate");
                break;
        }
    }
}