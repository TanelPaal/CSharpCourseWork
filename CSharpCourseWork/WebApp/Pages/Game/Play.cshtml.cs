﻿using ConsoleApp;
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
    
    public bool IsMovableGrid(int x, int y)
    {
        // Implement your logic to determine if the cell (x, y) is part of the Movable Grid
        return IsInsidePlayableArea(x, y);
    }
    
    private bool IsInsidePlayableArea(int x, int y)
    {
        if (GameState?.GameConfiguration == null)
        {
            return false;
        }
        
        int boardWidth = GameState.GameConfiguration.BoardSizeWidth;
        int boardHeight = GameState.GameConfiguration.BoardSizeHeight;

        int gridStartX = (boardWidth - 3) / 2;
        int gridStartY = (boardHeight - 3) / 2;
        int gridEndX = gridStartX + 2;
        int gridEndY = gridStartY + 2;

        return x >= gridStartX && x <= gridEndX && y >= gridStartY && y <= gridEndY;
    }
    
    
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