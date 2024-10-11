namespace GameBrain;

public class TicTacTwoBrain
{
    private EGamePiece[,] _gameBoard;
    private EGamePiece _nextMoveBy { get; set; } = EGamePiece.X;

    private GameConfiguration _gameConfiguration;

    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        _gameConfiguration = gameConfiguration;
        _gameBoard = new EGamePiece[_gameConfiguration.BoardSizeWidth, _gameConfiguration.BoardSizeHeight];
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


    public bool MakeAMove(int x, int y)
    {
        if (_gameBoard[x, y] != EGamePiece.Empty)
        {
            return false;
        }

        _gameBoard[x, y] = _nextMoveBy;
        
        // Check for win condition.
        if (CheckWinCondition())
        {
            Console.WriteLine($"{_nextMoveBy} wins!");
            return true;
        }
        
        // flip the next piece
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;

        return true;
    }

    public bool CheckWinCondition()
    {
        // Check rows
        for (int y = 0; y < DimY; y++)
        {
            for (int x = 0; x <= DimX - 3; x++)
            {
                if (_gameBoard[x, y] != EGamePiece.Empty &&
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
            for (int y = 0; y <= DimY - 3; y++)
            {
                if (_gameBoard[x, y] != EGamePiece.Empty &&
                    _gameBoard[x, y] == _gameBoard[x, y + 1] &&
                    _gameBoard[x, y] == _gameBoard[x, y + 2])
                {
                    return true;
                }
            }
        }
        
        // Check diagonals (top-left to bottom-right)
        for (int x = 0; x <= DimX - 3; x++)
        {
            for (int y = 0; y <= DimY - 3; y++)
            {
                if (_gameBoard[x, y] != EGamePiece.Empty &&
                    _gameBoard[x, y] == _gameBoard[x + 1, y + 1] &&
                    _gameBoard[x, y] == _gameBoard[x + 2, y + 2])
                {
                    return true;
                }
            }
        }

        // Check diagonals (bottom-left to top-right)
        for (int x = 0; x <= DimX - 3; x++)
        {
            for (int y = 2; y < DimY; y++)
            {
                if (_gameBoard[x, y] != EGamePiece.Empty &&
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
    }
}