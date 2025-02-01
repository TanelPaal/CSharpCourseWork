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

    public string GameId = "";
    public Play(IGameRepository gameRepository, IHubContext<GameHub> hubContext, GameService gameService)
    {
        _gameRepository = gameRepository;
        _hubContext = hubContext;
        _gameService = gameService;
    }
    
    public GameService _gameService { get; set; }
    public GameState? GameState { get; set; }
    public string GameError { get; set; } = "";

    public ConcurrentDictionary<string, Player>? Players { get; set; }
    
    public bool IsMovableGrid(int x, int y)
    {
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

    public async Task<IActionResult> OnPostAsync(string gameId, int x, int y, int oldX, int oldY, string action)
    {
        GameId = gameId;
        GameState = _gameRepository.GetSaveById(int.Parse(gameId));
        _gameBrain = new TicTacTwoBrain(GameState);
        var tempGameBrain = _gameBrain;
        Console.WriteLine(_gameService._gameSessions.Count);
        
        
        // Get the current player's piece before making any moves
        Console.WriteLine(gameId + HttpContext.Request.Query["username"]);
        var currentPlayer = _gameService.GetPlayer(gameId, HttpContext.Request.Query["username"]);
        // Only check turn validity if it's not an AI opponent move
        if (action != "opponentAIMove" && (currentPlayer == null || currentPlayer.Piece != GameState._nextMoveBy.ToString()))
        {
            GameError = "Not your turn!";
            Console.WriteLine("tried resubmitting");
            await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate", _gameBrain._gameState);
        
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return new JsonResult(new { success = true, gameState = GameState });
            }
            return RedirectToPage("./Play", new { gameId = GameId, username = HttpContext.Request.Query["username"] });
        }
        
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
                await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate", _gameBrain._gameState);
                Console.WriteLine("requesting clients to reload");
                break;

            case "moveGrid":
                output[0, 0] = 2;
                output[1, 0] = x;
                output[1, 1] = y;
                var inputResult = GameController.ProcessInput((output, true, false), tempGameBrain);
                if (inputResult == "success")
                {
                    _gameBrain = tempGameBrain;
                    _gameRepository.SaveGame(tempGameBrain._gameState, tempGameBrain._gameState.GameConfiguration.Name);
                    Console.WriteLine("sending gamestate update to " + GameId);
                    await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate", _gameBrain._gameState);
                }
                else
                {
                    GameError = inputResult;
                }

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
                await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate", _gameBrain._gameState);
                break;
            
            case "randomMove":
                var randomMove = _gameBrain.GetRandomValidMove();
                if (randomMove.HasValue)
                {
                    output[0, 0] = 1; // Place piece action
                    output[1, 0] = randomMove.Value.x;
                    output[1, 1] = randomMove.Value.y;
                    GameController.ProcessInput((output, true, false), tempGameBrain);
                    _gameBrain = tempGameBrain;
                    _gameRepository.SaveGame(tempGameBrain._gameState, tempGameBrain._gameState.GameConfiguration.Name);
                    await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate", _gameBrain._gameState);
                }
                break;
            case "opponentAIMove":
                // Store the current piece and player's piece
                var currentPiece = _gameBrain._gameState._nextMoveBy;
                var playerMakingMove = _gameService.GetPlayer(gameId, HttpContext.Request.Query["username"]);
    
                // Switch to the opposite piece of the player's piece (not the current game piece)
                var playerPiece = playerMakingMove.Piece == "X" ? EGamePiece.O : EGamePiece.X;
                _gameBrain._gameState._nextMoveBy = playerPiece;
                
                // Randomly choose between different movve types
                var random = new Random();
                var moveType = random.Next(3);

                bool moveMade = false;

                switch (moveType)
                {
                    case 0: // Place Piece
                        var randomOpponentMove = _gameBrain.GetRandomValidMove();
                        if (randomOpponentMove.HasValue)
                        {
                            output[0, 0] = 1; // Place piece action
                            output[1, 0] = randomOpponentMove.Value.x;
                            output[1, 1] = randomOpponentMove.Value.y;
                            GameController.ProcessInput((output, true, false), tempGameBrain);
                            moveMade = true;
                        }
                        break;
                    case 1: // Move Grid
                        var randomGridMove = _gameBrain.GetRandomGridMove();
                        if (randomGridMove.HasValue)
                        {
                            output[0, 0] = 2; // Move grid action
                            output[1, 0] = randomGridMove.Value.x;
                            output[1, 1] = randomGridMove.Value.y;
                            var result = GameController.ProcessInput((output, true, false), tempGameBrain);
                            moveMade = result == "success";
                        }
                        break;
                    case 2: // Move Piece
                        var randomPieceMove = _gameBrain.GetRandomPieceMove();
                        if (randomPieceMove.HasValue)
                        {
                            output[0, 0] = 3; // Move piece action
                            output[1, 0] = randomPieceMove.Value.newX;
                            output[1, 1] = randomPieceMove.Value.newY;
                            output[2, 0] = randomPieceMove.Value.oldX;
                            output[2, 1] = randomPieceMove.Value.oldY;
                            GameController.ProcessInput((output, true, true), tempGameBrain);
                            moveMade = true;
                        }
                        break;
                }
                
                // If the chosen move type failed, try placing a piece as fallback
                if (!moveMade)
                {
                    var fallbackMove = _gameBrain.GetRandomValidMove();
                    if (fallbackMove.HasValue)
                    {
                        output[0, 0] = 1; // Place piece action
                        output[1, 0] = fallbackMove.Value.x;
                        output[1, 1] = fallbackMove.Value.y;
                        GameController.ProcessInput((output, true, false), tempGameBrain);
                    }
                }

                _gameBrain = tempGameBrain;
                _gameRepository.SaveGame(tempGameBrain._gameState, tempGameBrain._gameState.GameConfiguration.Name);
                await _hubContext.Clients.Group(GameId).SendAsync("ReceiveGameStateUpdate", _gameBrain._gameState);
                break;
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            // Return JSON response for AJAX requests
            return new JsonResult(new { success = true, gameState = GameState });
        }
        
        if (_gameBrain.CheckWinCondition() || _gameBrain.IsBoardFull())
        {
            // Notify all players in the game about game over
            await _hubContext.Clients.Group(GameId).SendAsync("GameOver");
            return RedirectToPage("./GameOver", new { gameId = GameId, username = HttpContext.Request.Query["username"] });
        }

        return RedirectToPage("./Play", new { gameId = GameId, username = HttpContext.Request.Query["username"] });
    }
}