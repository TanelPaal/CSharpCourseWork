namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePiece _nextMoveBy { get; set; } = EGamePiece.X;
    public int[] _gameArea { get; set; }
 
    public GameConfiguration GameConfiguration { get; set; }

    public GameState(EGamePiece[][] gameBoard, GameConfiguration gameConfiguration, int[] gameArea)
    {
        GameBoard = gameBoard;
        GameConfiguration = gameConfiguration;
        _gameArea = gameArea;
    }


    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}