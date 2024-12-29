using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.GameServ;

namespace WebApp.Pages.Game;

public class ListGames : PageModel
{
    private IGameRepository _gameRepository;
    private readonly GameService _gameService;

    public ListGames(IGameRepository gameRepository, GameService gameService)
    {
        _gameRepository = gameRepository;
        _gameService = gameService;
    }

    public GameState gameSave { get; set; } = default!;

    public List<GameState> GameSaves { get; set; } = new List<GameState>();

    public void OnGet()
    {
        var gameNameList = _gameRepository.GetSaveNames();
        foreach (var gameName in gameNameList)
        {
            var gameSave = _gameRepository.GetSaveByName(gameName);
            GameSaves.Add(gameSave);
        }
    }
    public bool CanJoinGame(string gameId)
    {
        return _gameService.GetPlayerCount(gameId) < 2;
    }
}