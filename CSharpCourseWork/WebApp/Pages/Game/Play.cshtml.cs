using ConsoleApp;
using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game;

public class Play : PageModel
{
    private IGameRepository _gameRepository;

    private TicTacTwoBrain? _gameBrain { get; set; }
    public string GameId = "";

    public Play(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;

    }

    public GameState? GameState { get; set; }
    
    
    public void OnGet(string gameId)
    {
        GameId = gameId;
        GameState = _gameRepository.GetSaveById(int.Parse(gameId));
        _gameBrain = new TicTacTwoBrain(GameState);
        Console.WriteLine(_gameBrain);
    }

    public IActionResult OnPost(string gameId, int x, int y, int oldX, int oldY, string action)
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
                break;

            case "moveGrid":
                output[0, 0] = 2;
                output[1, 0] = x;
                output[1, 1] = y;
                GameController.ProcessInput((new int[,] { { x, y } }, true, false), tempGameBrain);
                _gameBrain = tempGameBrain;
                _gameRepository.SaveGame(tempGameBrain._gameState, tempGameBrain._gameState.GameConfiguration.Name);

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
                break;

        }

        return Page();
    }
}