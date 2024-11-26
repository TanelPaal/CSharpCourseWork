using GameBrain;

namespace DAL;

using System.Text.Json;
using System.Collections.Generic;

public class JsonGameRespository: IGameRepository
{
    public void SaveGame(GameState gameState, string gameSaveName)
    {

        string jsonStateString = gameState.ToString();
        
        File.WriteAllText(FileHelper.BasePath +
                          gameSaveName + "_" +
                          DateTime.Now.ToString("yyyy-MM-dd_HH-mm") +
                          FileHelper.GameExtension, jsonStateString);
    }
    
    public GameState GetSaveByName(string gameSaveName)
    {
        var filePath = FileHelper.BasePath + gameSaveName + FileHelper.GameExtension;

        var configString = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
        };

        // Console.WriteLine("JSON String: " + configString);
        var gameData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(configString!)!;

        var gameConfigDict = gameData["GameConfiguration"].Deserialize<Dictionary<string, JsonElement>>(options)!;
        var tempGameConfig = new GameConfiguration
            {
                Name = gameConfigDict["Name"].GetString(),
                BoardSizeWidth = gameConfigDict["BoardSizeWidth"].GetInt32(),
                BoardSizeHeight = gameConfigDict["BoardSizeHeight"].GetInt32(),
                MovePieceAfterNMoves = gameConfigDict["MovePieceAfterNMoves"].GetInt32(),
                PieceLimit = gameConfigDict["PieceLimit"].GetInt32()
            };
        var gameArea = JsonSerializer.Deserialize<int[]>(((JsonElement)gameData["_gameArea"]).GetRawText());

        var gameState = new GameState(
            // Extract GameBoard and cast to EGamePiece[][]
            (gameData["GameBoard"]).Deserialize<EGamePiece[][]>(),
            // Extract GameConfiguration and cast to GameConfiguration object
            tempGameConfig,
            // Extract _gameArea and cast to int[]
            gameArea,
            // Extract _nextMoveBy and cast to EGamePiece
            (EGamePiece)gameData["_nextMoveBy"].GetInt32(),
            // Extract _xTurnCount and cast to int
            gameData["_xTurnCount"].GetInt32(),
            // Extract _oTurnCount and cast to int
            gameData["_oTurnCount"].GetInt32()
        );


        return gameState;
    }
    
    public bool DoesRootFolderContainSaves()
    {
        if (Directory.Exists(FileHelper.BasePath))
        {
            if (Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension).ToList().Count == 0)
            {
                return true;
            }
        }

        return false;
    }
    
    public List<string> GetSaveNames()
    {
        if (FileHelper.RootFolderGenerator() || FileHelper.DoesRootFolderContainConfigs())
        {
            return [];
        }

        return Directory
            .GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(
                    Path.GetFileNameWithoutExtension(fullFileName)
                )
            )
            .ToList();
        //load existing saves
    }
}