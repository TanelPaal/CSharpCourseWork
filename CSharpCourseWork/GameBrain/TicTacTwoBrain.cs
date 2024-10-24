namespace GameBrain;
using System.Text.Json;

public class TicTacTwoBrain
{
    public  GameState _gameState;

    // public int[] GameArea => _gameArea;
    // public int[]  _gameArea { get; set; }

    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        var gameBoard = new EGamePiece[gameConfiguration.BoardSizeWidth][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[gameConfiguration.BoardSizeHeight];
        }

        // Calculate the center position
        int centerX = (int)Math.Floor((double)gameConfiguration.BoardSizeWidth / 2);
        int centerY = (int)Math.Floor((double)gameConfiguration.BoardSizeHeight / 2);
        int[] _gameArea = { centerX, centerY };
        Console.WriteLine($"{centerX}, {centerY}");


        _gameState = new GameState(
            gameBoard,
            gameConfiguration,
            _gameArea
        );
    }

    public TicTacTwoBrain(GameState gameState)
    {
        _gameState = gameState;
    }

    public string GetGameStateJson()
    {

        var gameStateJson = new GameState2(_gameState).ToString();
        
        return gameStateJson!;
        
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
    }


    public bool MovePlayableArea(int x, int y)
    {

        if (Math.Abs(_gameState._gameArea[0] - x) > 1 && Math.Abs(_gameState._gameArea[1] - y) > 1)
        {
            Console.WriteLine("Idiot, you can't move it that far. You saw what happened when we pushed to Moscow!");
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
            
            Console.WriteLine($"Playable area moved to ({x}, {y}).");
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