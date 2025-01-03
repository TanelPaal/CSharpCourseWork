using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.GameServ;

namespace WebApp.Pages.Game;

public class GameOver : PageModel
{
    private readonly GameService _gameService;
    private readonly IGameRepository _gameRepository;

    public GameOver(GameService gameService, IGameRepository gameRepository)
    {
        _gameService = gameService;
        _gameRepository = gameRepository;
    }

    public string Winner { get; set; } = "";
    public string Loser { get; set; } = "";

    public async Task OnGet(string gameId)
    {
        try 
        {
            var gamePlayers = _gameService._gameSessions[gameId];
            var gameState = _gameRepository.GetSaveById(int.Parse(gameId));
            var winningPiece = gameState._nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;

            foreach (var player in gamePlayers)
            {
                if (player.Value.Piece == winningPiece.ToString())
                {
                    Winner = player.Value.Username;
                }
                else
                {
                    Loser = player.Value.Username;
                }
            }

            // Store the winner and loser information in TempData
            TempData["Winner"] = Winner;
            TempData["Loser"] = Loser;

            // Only clean up the game session if we haven't already
            if (!string.IsNullOrEmpty(Winner) && !string.IsNullOrEmpty(Loser))
            {
                await _gameService.EndGame(gameId);
            }
        }
        catch
        {
            // If the game session is already cleaned up, try to get the data from TempData
            Winner = TempData["Winner"]?.ToString() ?? "";
            Loser = TempData["Loser"]?.ToString() ?? "";
        }
    }
}