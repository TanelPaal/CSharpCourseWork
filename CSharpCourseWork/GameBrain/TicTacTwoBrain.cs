namespace GameBrain;

public class TicTacTwoBrain
{
    private EGamePiece[,] _gameBoard;
    private EGamePiece _nextMoveBy { get; set; } = EGamePiece.X;
    private GameConfiguration _gameConfiguration;
    private int _xTurnCount = 0;
    private int _oTurnCount = 0;
    private int _xPieceCount = 0; // Track the number of X pieces
    private int _oPieceCount = 0; // Track the number of O pieces

    public int[] GameArea => _gameArea;
    public int[]  _gameArea { get; set; }
    
    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        _gameConfiguration = gameConfiguration;
        _gameBoard = new EGamePiece[_gameConfiguration.BoardSizeWidth, _gameConfiguration.BoardSizeHeight];
        // Calculate the center position
        int centerX = (int)Math.Floor((double)_gameConfiguration.BoardSizeWidth / 2);
        int centerY = (int)Math.Floor((double)_gameConfiguration.BoardSizeHeight / 2);
        Console.WriteLine($"{centerX}, {centerY}");
        _gameArea = new int[] { centerX, centerY };
    }
    
    public EGamePiece[,] GameBoard
    {
        get => GetBoard();
        private set => _gameBoard = value;
    }

    public int DimX => _gameBoard.GetLength(0);
    public int DimY => _gameBoard.GetLength(1);
    
    private EGamePiece[,] GetBoard()
    {
        var copyOfBoard = new EGamePiece[_gameBoard.GetLength(0), _gameBoard.GetLength(1)];
        for (var x = 0; x < _gameBoard.GetLength(0); x++)
        {
            for (var y = 0; y < _gameBoard.GetLength(1); y++)
            {
                copyOfBoard[x, y] = _gameBoard[x, y];
            }
        }

        return copyOfBoard;
    }

    public bool MovePlayableArea(int x, int y)
    {

        if (Math.Abs(_gameArea[0] - x) > 1 && Math.Abs(_gameArea[1] - y) > 1)
        {
            Console.WriteLine("Idiot, you can't move it that far. You saw what happened when we pushed to Moscow!");
            return false;
        }
        
        // Check if the move is allowed based on the turn counts
        if (_gameConfiguration.MovePieceAfterNMoves > 0 &&
            (_xTurnCount + _oTurnCount) < _gameConfiguration.MovePieceAfterNMoves)
        {
            Console.WriteLine("Not enough moves made to move the playable area.");
            return false;
        }
        
        if (PlayableAreaValidMove(x, y))
        {

            if (_nextMoveBy == EGamePiece.X)
            {
                _xTurnCount++;
            }
            else
            {
                _oTurnCount++;
            }

            _gameArea = [x, y];


            _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;


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

        if (_gameBoard[pieceX, pieceY] == EGamePiece.Empty)
        {
            Console.WriteLine("The cell you are trying to move doesnt exist");
            return false;  
        }
        if (_gameBoard[pieceX, pieceY] != _nextMoveBy)
        {
            Console.WriteLine("You can only move your own pieces.");
            return false;
        }
        if (_gameBoard[x, y] != EGamePiece.Empty)
        {
            Console.WriteLine("Cell is already occupied.");
            return false;
        }

        _gameBoard[pieceX, pieceY] = EGamePiece.Empty;
        _gameBoard[x, y] = _nextMoveBy;

        if (_nextMoveBy == EGamePiece.X)
        {
            _xTurnCount++;
        }
        else
        {
            _oTurnCount++;
        }
        
        // Check for win condition.
        if (CheckWinCondition())
        {
            Console.WriteLine($"{_nextMoveBy} wins!");
            return true;
        }
        
        // Check for tie condition.
        if (IsBoardFull())
        {
            Console.WriteLine("It's a tie!");
            return true;
        }

        // Console.WriteLine(GetBoard());
        
        // flip the next piece
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;

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
        int centerX = _gameArea[0];
        int centerY = _gameArea[1];
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
        Console.WriteLine($"{_nextMoveBy} made a move."); //Console.WriteLine($"{_nextMoveBy} is making a move at ({x}, {y})");
        
        if (!ValidMove(x, y))
        {
            Console.WriteLine("Invalid move.");
            return false;
        }
        if (_gameBoard[x, y] != EGamePiece.Empty)
        {
            Console.WriteLine("Cell is already occupied.");
            return false;
        }

        _gameBoard[x, y] = _nextMoveBy;

        if (_nextMoveBy == EGamePiece.X)
        {
            _xTurnCount++;
        }
        else
        {
            _oTurnCount++;
        }
        
        // Check for win condition.
        if (CheckWinCondition())
        {
            Console.WriteLine($"{_nextMoveBy} wins!");
            return true;
        }
        
        // Check for tie condition.
        if (IsBoardFull())
        {
            Console.WriteLine("It's a tie!");
            return true;
        }
        
        // flip the next piece
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;

        return true;
    }

    public bool IsBoardFull()
    {
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (_gameBoard[x, y] == EGamePiece.Empty)
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
                    _gameBoard[x, y] != EGamePiece.Empty &&
                    _gameBoard[x, y] == _gameBoard[x + 1, y] &&
                    _gameBoard[x, y] == _gameBoard[x + 2, y])
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
                    _gameBoard[x, y] != EGamePiece.Empty &&
                    _gameBoard[x, y] == _gameBoard[x, y + 1] &&
                    _gameBoard[x, y] == _gameBoard[x, y + 2])
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
                    _gameBoard[x, y] != EGamePiece.Empty &&
                    _gameBoard[x, y] == _gameBoard[x + 1, y + 1] &&
                    _gameBoard[x, y] == _gameBoard[x + 2, y + 2])
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
                    _gameBoard[x, y] != EGamePiece.Empty &&
                    _gameBoard[x, y] == _gameBoard[x + 1, y - 1] &&
                    _gameBoard[x, y] == _gameBoard[x + 2, y - 2])
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public void ResetGame()
    {
        _gameBoard = new EGamePiece[_gameBoard.GetLength(0), _gameBoard.GetLength(1)];
        _nextMoveBy = EGamePiece.X;
        _xTurnCount = 0;
        _oTurnCount = 0;
        Console.WriteLine("Game has been reset.");
    }
}