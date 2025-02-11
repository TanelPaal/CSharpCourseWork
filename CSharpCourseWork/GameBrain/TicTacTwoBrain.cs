﻿namespace GameBrain;
using System.Text.Json;

public class TicTacTwoBrain
{
    public  GameState _gameState;

    public TicTacTwoBrain(GameState gameState)
    {
        _gameState = gameState;
    }

    public GameState GetGameState()
    {

        var gameStateJson = _gameState.ToString();

        return _gameState;

    }

    public string GetGameConfigName()
    {
        return _gameState.GameConfiguration.Name;
    }
    public EGamePiece[][] GameBoard
    {
        get => GetBoard();
        private set => _gameState.GameBoard = value;
    }

    public int DimX => _gameState.GameBoard.Length;
    public int DimY => _gameState.GameBoard[0].Length;
    
    private EGamePiece[][] GetBoard()
    {
        var copyOfBoard = new EGamePiece[_gameState.GameBoard.GetLength(0)][];
        //, _gameState.GameBoard.GetLength(1)];
        for (var x = 0; x < _gameState.GameBoard.Length; x++)
        {
            copyOfBoard[x] = new EGamePiece[_gameState.GameBoard[x].Length];
            for (var y = 0; y < _gameState.GameBoard[x].Length; y++)
            {
                copyOfBoard[x][y] = _gameState.GameBoard[x][y];
            }
        }

        return copyOfBoard;
    }
    
    private void IncrementTurnCount()
    {
        if (_gameState._nextMoveBy == EGamePiece.X)
        {
            _gameState._xTurnCount++;
        }
        else
        {
            _gameState._oTurnCount++;
        }
    }

    private void GetNextMoveBy()
    {
        _gameState._nextMoveBy = _gameState._nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        Console.WriteLine("switching sides");
    }

    public bool MovePlayableArea(int x, int y)
    {

        if (Math.Abs(_gameState._gameArea[0] - x) > 1 && Math.Abs(_gameState._gameArea[1] - y) > 1)
        {
            Console.WriteLine("Invalid move: The playable area can only be moved one space at a time.");
            return false;
        }
        // Check if the move is allowed based on the turn counts
        if (_gameState.GameConfiguration.MovePieceAfterNMoves > 0 &&
             (_gameState._xTurnCount + _gameState._oTurnCount) < _gameState.GameConfiguration.MovePieceAfterNMoves)
        {
            Console.WriteLine("Not enough moves made to move the playable area.");
            return false;
        }
        
        if (PlayableAreaValidMove(x, y))
        {

            IncrementTurnCount();

            _gameState._gameArea = [x, y];
            
            GetNextMoveBy();
            
            //Console.WriteLine($"Playable area moved to ({x}, {y}).");
            return true;
        }
        
        Console.WriteLine("Invalid move for Playable Area.");
        return false;
    }

    public bool MoveExistingPiece(int x, int y, int pieceX, int pieceY)
    {
        if (!ValidMove(x, y))
        {
            Console.WriteLine("Invalid move.");
            return false;
        }

        if (_gameState.GameBoard[pieceX][pieceY] == EGamePiece.Empty)
        {
            Console.WriteLine("The cell you are trying to move doesnt exist");
            return false;  
        }
        if (_gameState.GameBoard[pieceX][pieceY] != _gameState._nextMoveBy)
        {
            Console.WriteLine("You can only move your own pieces.");
            return false;
        }
        if (_gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            Console.WriteLine("Cell is already occupied.");
            return false;
        }

        _gameState.GameBoard[pieceX][pieceY] = EGamePiece.Empty;
        _gameState.GameBoard[x][y] = _gameState._nextMoveBy;

        IncrementTurnCount();
        
        // Check for win condition.
        if (CheckWinCondition())
        {
            Console.WriteLine($"{_gameState._nextMoveBy} wins!");
            return true;
        }
        // Check for tie condition.
        if (IsBoardFull())
        {
            Console.WriteLine("It's a tie!");
            return true;
        }
        // Console.WriteLine(GetBoard());
        GetNextMoveBy();

        return true;
    }

    public bool PlayableAreaValidMove(int x, int y)
    {
        if (x < DimX - 1 && x > 0 && y < DimY - 1 && y > 0)
        {
            return true;
        }
        return false;
    }
    
    public bool IsInsidePlayableArea(int x, int y)
    {
        int centerX = _gameState._gameArea[0];
        int centerY = _gameState._gameArea[1];
        return x >= centerX - 1 && x <= centerX + 1 && y >= centerY - 1 && y <= centerY + 1;
    }
    
    
    public bool ValidMove(int x, int y)
    {
        if (x < DimX && x > -1 && y < DimY && y > -1 && IsInsidePlayableArea(x, y))
        {
            return true;
        }

        return false;
    }


    public bool MakeAMove(int x, int y)
    {
        Console.WriteLine($"{_gameState._nextMoveBy} made a move."); //Console.WriteLine($"{_nextMoveBy} is making a move at ({x}, {y})");
        
        if (!ValidMove(x, y))
        {
            Console.WriteLine("Invalid move.");
            return false;
        }
        if (_gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            Console.WriteLine("Cell is already occupied.");
            return false;
        }

        _gameState.GameBoard[x][y] = _gameState._nextMoveBy;

        IncrementTurnCount();
        
        // Check for win condition.
        if (CheckWinCondition())
        {
            Console.WriteLine($"{_gameState._nextMoveBy} wins!");
            return true;
        }
        
        // Check for tie condition.
        if (IsBoardFull())
        {
            Console.WriteLine("It's a tie!");
            return true;
        }

        GetNextMoveBy();

        return true;
    }

    public bool IsBoardFull()
    {
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (_gameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool CheckWinCondition()
    {
        // Check rows
        for (int y = 0; y < DimY; y++)
        {
            for (int x = 0; x <= DimX; x++)
            {
                if (IsInsidePlayableArea(x, y) && 
                    IsInsidePlayableArea(x + 1, y) && 
                    IsInsidePlayableArea(x + 2, y) &&
                    _gameState.GameBoard[x][y] != EGamePiece.Empty &&
                    _gameState.GameBoard[x][y] == _gameState.GameBoard[x + 1][y] &&
                    _gameState.GameBoard[x][y] == _gameState.GameBoard[x + 2][y])
                {
                    return true;
                }
            }
        }
        
        // Check columns
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y <= DimY; y++)
            {
                if (IsInsidePlayableArea(x, y) && 
                    IsInsidePlayableArea(x, y + 1) && 
                    IsInsidePlayableArea(x, y + 2) &&
                    _gameState.GameBoard[x][y] != EGamePiece.Empty &&
                    _gameState.GameBoard[x][y] == _gameState.GameBoard[x][y + 1] &&
                    _gameState.GameBoard[x][y] == _gameState.GameBoard[x][y + 2])
                {
                    return true;
                }
            }
        }
        
        // Check diagonals (top-left to bottom-right)
        for (int x = 0; x <= DimX; x++)
        {
            for (int y = 0; y <= DimY; y++)
            {
                if (IsInsidePlayableArea(x, y) && 
                    IsInsidePlayableArea(x + 1, y + 1) && 
                    IsInsidePlayableArea(x + 2, y + 2) &&
                    _gameState.GameBoard[x][y] != EGamePiece.Empty &&
                    _gameState.GameBoard[x][y] == _gameState.GameBoard[x + 1][y + 1] &&
                    _gameState.GameBoard[x][y] == _gameState.GameBoard[x + 2][y + 2])
                {
                    return true;
                }
            }
        }

        // Check diagonals (bottom-left to top-right)
        for (int x = 0; x <= DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (IsInsidePlayableArea(x, y) && 
                    IsInsidePlayableArea(x + 1, y - 1) && 
                    IsInsidePlayableArea(x + 2, y - 2) &&
                    _gameState.GameBoard[x][y] != EGamePiece.Empty &&
                    _gameState.GameBoard[x][y] == _gameState.GameBoard[x + 1][y - 1] &&
                    _gameState.GameBoard[x][y] == _gameState.GameBoard[x + 2][ y - 2])
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public (int x, int y)? GetRandomValidMove()
    {
        var random = new Random();
        var validMoves = new List<(int x, int y)>();

        // Collect all valid moves
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (GameBoard[x][y] == EGamePiece.Empty && IsInsidePlayableArea(x, y))
                {
                    validMoves.Add((x, y));
                }
            }
        }

        // Return random valid move if available
        if (validMoves.Count > 0)
        {
            return validMoves[random.Next(validMoves.Count)];
        }

        return null;
    }
    
    public (int x, int y)? GetRandomGridMove()
    {
        var random = new Random();
        var validMoves = new List<(int x, int y)>();

        // Get current grid position
        int currentX = _gameState._gameArea[0];
        int currentY = _gameState._gameArea[1];

        // Check all adjacent positions
        for (int x = currentX - 1; x <= currentX + 1; x++)
        {
            for (int y = currentY - 1; y <= currentY + 1; y++)
            {
                if (PlayableAreaValidMove(x, y))
                {
                    validMoves.Add((x, y));
                }
            }
        }

        // Return random valid move if available
        if (validMoves.Count > 0)
        {
            return validMoves[random.Next(validMoves.Count)];
        }

        return null;
    }

    public (int newX, int newY, int oldX, int oldY)? GetRandomPieceMove()
    {
        var random = new Random();
        var validMoves = new List<(int newX, int newY, int oldX, int oldY)>();

        // Only allow piece movement after certain number of moves
        if (_gameState.GameConfiguration.MovePieceAfterNMoves > 0 &&
            (_gameState._xTurnCount + _gameState._oTurnCount) < _gameState.GameConfiguration.MovePieceAfterNMoves)
        {
            return null;
        }

        // Find all pieces of the current player
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (_gameState.GameBoard[x][y] == _gameState._nextMoveBy)
                {
                    // Check all possible destinations
                    for (int newX = 0; newX < DimX; newX++)
                    {
                        for (int newY = 0; newY < DimY; newY++)
                        {
                            if (_gameState.GameBoard[newX][newY] == EGamePiece.Empty && 
                                IsInsidePlayableArea(newX, newY))
                            {
                                validMoves.Add((newX, newY, x, y));
                            }
                        }
                    }
                }
            }
        }

        if (validMoves.Count > 0)
        {
            return validMoves[random.Next(validMoves.Count)];
        }

        return null;
    }
    
    // Not implemented/ not needed maybe
    public void ResetGame()
    {
        var gameBoard = new EGamePiece[_gameState.GameConfiguration.BoardSizeWidth][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[_gameState.GameConfiguration.BoardSizeHeight];
        }

        _gameState.GameBoard = gameBoard;
        _gameState._nextMoveBy = EGamePiece.X;
        _gameState._xTurnCount = 0;
        _gameState._oTurnCount = 0;
        Console.WriteLine("Game has been reset.");
    }
}