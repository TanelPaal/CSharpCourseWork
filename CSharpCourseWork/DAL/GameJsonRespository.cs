using GameBrain;

namespace DAL;

using System.Text.Json;
using System.Collections.Generic;
public class GameJsonRespository: IGameRepository
{
    public void SaveGame(string jsonStateString, string gameSaveName)
    {
        File.WriteAllText(FileHelper.BasePath +
                          gameSaveName + "_" +
                          DateTime.Now.ToString("yyyy-MM-dd_HH-mm") +
                          FileHelper.GameExtension, jsonStateString);


    }
    
    public void SaveConfig(string jsonStateString, string gameConfigName)
    {
        File.WriteAllText(FileHelper.BasePath +
                          gameConfigName + "_" +
                          DateTime.Now.ToString("yyyy-MM-dd_HH-mm") +
                          FileHelper.ConfigExtension, jsonStateString);


    }


    public GameState GetSaveByName(string gameSaveName)
    {
        var filePath = FileHelper.BasePath + gameSaveName + FileHelper.GameExtension;
        
        var configString = File.ReadAllText(filePath);

        // Deserialize into the intermediate DTO with default values;
        // var gameSaveDto = JsonSerializer.Deserialize<GameStateDTO>(configString, new JsonSerializerOptions { IncludeFields = true, PropertyNameCaseInsensitive = true });

        // Manually map the int[][] GameBoard to EGamePiece[][]



        var options = new JsonSerializerOptions 
        { 
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
        };
        GameStateDTO GameStatedto;
        GameState gameStateTemp;

        Console.WriteLine("JSON String: " + configString);
        var test = JsonSerializer.Deserialize<Dictionary<string, object>>(configString!, options)!;
        GameStatedto = new GameStateDTO(test);
        gameStateTemp = GameStatedto.ToGameState();

        Console.WriteLine(gameStateTemp.ToString()); // For debugging





        
        //Console.WriteLine(gameSave!["_nextMoveBy"].GetInt32());
        
        

        var gameBoard2 = new EGamePiece[(int)2][];
        for (var x = 0; x < gameBoard2.Length; x++)
        {
            gameBoard2[x] = new EGamePiece[(int)3];
        }
        int[] _gameArea = { 1, 2 };

        var gameConfig2 = new GameConfiguration();

        
        GameState gameStateSave = new GameState(
            gameBoard2,
            gameConfig2,
            _gameArea
        );
        return gameStateTemp;
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