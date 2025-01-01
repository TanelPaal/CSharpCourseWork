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

    public void OnGet(string gameId)
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

        // Clean up the game session
        _gameService.EndGame(gameId);
    }
}