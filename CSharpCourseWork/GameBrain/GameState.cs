using System.Text.Json.Serialization;

namespace GameBrain;

using System.Text.Json;


public class GameState2
{
    [JsonPropertyName("GameBoard")]
    public int[][] GameBoard { get; set; }
    [JsonPropertyName("_nextMoveBy")]
    public int _nextMoveBy { get; set; }
    [JsonPropertyName("_gameArea")]
    public int[] _gameArea { get; set; }
    [JsonPropertyName("_xTurnCount")]
    public int _xTurnCount { get; set; }
    [JsonPropertyName("_oTurnCount")]
    public int _oTurnCount { get; set; }
    
    // Game Configuration properties
    [JsonPropertyName("Name")]
    public string Name { get; set; }
    [JsonPropertyName("BoardSizeWidth")]
    public int BoardSizeWidth { get; set; }
    [JsonPropertyName("BoardSizeHeight")]
    public int BoardSizeHeight { get; set; }
    [JsonPropertyName("MovePieceAfterNMoves")]
    public int MovePieceAfterNMoves { get; set; }
    [JsonPropertyName("PieceLimit")]
    public int PieceLimit { get; set; }
    
    public GameState2(GameState gameState)
    {
    
        {
            GameBoard = gameState.GameBoard.Select(row => row.Select(piece => (int)piece).ToArray()).ToArray(); // Assuming EGamePiece is an enum
            _nextMoveBy = (int)gameState._nextMoveBy; // Cast to int
            _gameArea = gameState._gameArea;
            _xTurnCount = gameState._xTurnCount;
            _oTurnCount = gameState._oTurnCount;
    
            // Flattening the GameConfiguration properties
            Name = gameState.GameConfiguration.Name;
            BoardSizeWidth = gameState.GameConfiguration.BoardSizeWidth;
            BoardSizeHeight = gameState.GameConfiguration.BoardSizeHeight;
            MovePieceAfterNMoves = gameState.GameConfiguration.MovePieceAfterNMoves;
            PieceLimit = gameState.GameConfiguration.PieceLimit;
        };
    }
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true});
    }
}

public class GameStateDTO
{

    public Dictionary<string, object> Values { get; set; }

    // Parameterless constructor
    public GameStateDTO()
    {
        Values = new Dictionary<string, object>();
    }

    // Constructor for initialization
    [JsonConstructor]
    public GameStateDTO(Dictionary<string, object> values)
    {
        Values = values;
    }

    // Method to convert Dictionary to GameState
    public GameState ToGameState()
    {
        // Extract values and convert to appropriate types
        Console.WriteLine(Values["_nextMoveBy"]);
        var gameBoard = (JsonElement)Values["GameBoard"];
        var nextMoveBy = (EGamePiece)Convert.ToInt32(Values["_nextMoveBy"].ToString());
        var gameArea = ((JsonElement)Values["_gameArea"]).EnumerateArray().Select(v => v.GetInt32()).ToArray();
        var xTurnCount = Convert.ToInt32(Values["_xTurnCount"].ToString());
        var oTurnCount = Convert.ToInt32(Values["_oTurnCount"].ToString());

        // Extract GameConfiguration properties
        var gameConfig = new GameConfiguration
        {
            Name = Values["Name"].ToString(),
            BoardSizeWidth = Convert.ToInt32(Values["BoardSizeWidth"].ToString()),
            BoardSizeHeight = Convert.ToInt32(Values["BoardSizeHeight"].ToString()),
            MovePieceAfterNMoves = Convert.ToInt32(Values["MovePieceAfterNMoves"].ToString()),
            PieceLimit = Convert.ToInt32(Values["PieceLimit"].ToString())
        };

        // Convert JsonElement GameBoard to EGamePiece[][]
        var boardArray = JsonSerializer.Deserialize<int[][]>(gameBoard.GetRawText());
        var eGameBoard = new EGamePiece[boardArray!.Length][];
        for (int i = 0; i < boardArray.Length; i++)
        {
            eGameBoard[i] = new EGamePiece[boardArray[i].Length];
            for (int j = 0; j < boardArray[i].Length; j++)
            {
                eGameBoard[i][j] = (EGamePiece)boardArray[i][j];
            }
        }

        return new GameState(
            eGameBoard,
            gameConfig,
            gameArea,
            nextMoveBy,
            xTurnCount,
            oTurnCount
        );
    }

    // public GameStateDTO(GameState gameState)
    // {
    //
    //     {
    //         GameBoard = gameState.GameBoard.Select(row => row.Select(piece => (int)piece).ToArray()).ToArray(); // Assuming EGamePiece is an enum
    //         _nextMoveBy = (int)gameState._nextMoveBy; // Cast to int
    //         _gameArea = gameState._gameArea;
    //         _xTurnCount = gameState._xTurnCount;
    //         _oTurnCount = gameState._oTurnCount;
    //
    //         // Flattening the GameConfiguration properties
    //         Name = gameState.GameConfiguration.Name;
    //         BoardSizeWidth = gameState.GameConfiguration.BoardSizeWidth;
    //         BoardSizeHeight = gameState.GameConfiguration.BoardSizeHeight;
    //         MovePieceAfterNMoves = gameState.GameConfiguration.MovePieceAfterNMoves;
    //         PieceLimit = gameState.GameConfiguration.PieceLimit;
    //     };
    // }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true});
    }
}


public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePiece _nextMoveBy { get; set; } = EGamePiece.X;
    public int[] _gameArea { get; set; }
    public int _xTurnCount { get; set; } = 0;
    public int _oTurnCount { get; set; } = 0;
 
    public GameConfiguration GameConfiguration { get; set; }

    public GameState(EGamePiece[][] gameBoard, GameConfiguration gameConfiguration, int[] gameArea)
    {
        GameBoard = gameBoard;
        GameConfiguration = gameConfiguration;
        _gameArea = gameArea;
    }

    public GameState(EGamePiece[][] gameBoard, GameConfiguration gameConfiguration, int[] gameArea, EGamePiece nextMoveBy, int xTurnCount, int oTurnCount)
    {
        GameBoard = gameBoard;
        GameConfiguration = gameConfiguration;
        _gameArea = gameArea;
        _nextMoveBy = nextMoveBy;
        _xTurnCount = xTurnCount;
        _oTurnCount = oTurnCount;
    }
    
    
    

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true});
    }


}