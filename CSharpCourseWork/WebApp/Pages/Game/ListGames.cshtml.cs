using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game;

public class ListGames : PageModel
{
    private IGameRepository _gameRepository;

    public ListGames(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public GameState gameSave { get; set; } = default!;

    public List<GameState> GameSaves { get; set; } = new List<GameState>();

    public void OnGet()
    {
        var gameNameList = _gameRepository.GetSaveNames();
        Console.WriteLine($"Fetched {gameNameList.Count} game names from the database.");
        foreach (var gameName in gameNameList)
        {
            Console.WriteLine(gameName);
            var gameSave = _gameRepository.GetSaveByName(gameName);
            Console.WriteLine(gameSave.Id);
            GameSaves.Add(gameSave);
            
            Console.WriteLine(GameSaves.Count);
        }
    }
}