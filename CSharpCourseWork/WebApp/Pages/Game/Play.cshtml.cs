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
    
    
    public IActionResult  OnGet(string gameId)
    {
        GameId = gameId;
        GameState = _gameRepository.GetSaveById(int.Parse(gameId));
        _gameBrain = new TicTacTwoBrain(GameState);
        return Page();
    }

    public IActionResult OnPost(string gameId, int x, int y, string action)
    {
        Console.WriteLine(gameId);
        Console.WriteLine("hello");
        GameId = gameId;
        GameState = _gameRepository.GetSaveById(int.Parse(gameId));
        _gameBrain = new TicTacTwoBrain(GameState);

        int[,] output = new int[4, 2];

        switch (action)
        {
            case "place":
                

                output[0, 0] = 1;
                output[1, 0] = x;
                output[1, 1] = y;
                GameController.ProcessInput((output, true, false), _gameBrain);
                break;
            case "moveGrid":
                output[0, 0] = 2;
                output[1, 0] = x;
                output[1, 1] = y;
                GameController.ProcessInput((new int[,] { { x, y } }, true, false), _gameBrain);
                break;
            case "movePiece":
                // Implement logic to move a piece
                break;
            case "save":
                _gameRepository.SaveGame(GameState, GameState.Id.ToString());
                break;
        }

        return Page();
    }
}