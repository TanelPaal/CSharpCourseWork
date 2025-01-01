using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.GameServ;

namespace WebApp.Pages.Game;

public class GameOver : PageModel
{
    private readonly GameService _gameService;

    public GameOver(GameService gameService)
    {
        _gameService = gameService;
    }

    public string Winner { get; set; } = "";
    public string Loser { get; set; } = "";

    public void OnGet(string gameId)
    {
        var gamePlayers = _gameService._gameSessions[gameId];
        
        foreach (var player in gamePlayers)
        {
            if (player.Value.Piece == "X")
            {
                Winner = player.Value.Username;
            }
            else
            {
                Loser = player.Value.Username;
            }
        }
    }
}