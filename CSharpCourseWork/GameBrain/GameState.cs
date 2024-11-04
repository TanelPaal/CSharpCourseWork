﻿using System.Text.Json.Serialization;

namespace GameBrain;

using System.Text.Json;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePiece _nextMoveBy { get; set; } = EGamePiece.X;
    public int[] _gameArea { get; set; }
    public int _xTurnCount { get; set; } = 0;
    public int _oTurnCount { get; set; } = 0;
    public GameConfiguration GameConfiguration { get; set; }
    
    [JsonConstructor]
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
        return JsonSerializer.Serialize(this);
    }


}