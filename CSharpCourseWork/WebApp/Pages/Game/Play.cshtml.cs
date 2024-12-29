using ConsoleApp;
using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using WebApp.Hubs;

namespace WebApp.Pages.Game;

public class Play : PageModel
{
    private IGameRepository _gameRepository;
    private readonly IHubContext<GameHub> _hubContext;
    private TicTacTwoBrain? _gameBrain { get; set; }
    
    public string GameId = "";
    public string Username = "";

    public Play(IGameRepository gameRepository, IHubContext<GameHub> hubContext)
    {
        _gameRepository = gameRepository;
        _hubContext = hubContext;
    }

    public GameState? GameState { get; set; }
    
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

        Username = HttpContext.Session.GetString("Username")!;
        Console.WriteLine(Username);
    }

    public async void  OnPost(string gameId, int x, int y, int oldX, int oldY, string action)
    {
        GameId = gameId;
        GameState = _gameRepository.GetSaveById(int.Parse(gameId));
        _gameBrain = new TicTacTwoBrain(GameState);
        Username = HttpContext.Session.GetString("Username")!;
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
                await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate");

                break;

            case "movePiece":
                // Implement logic to move a piece
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