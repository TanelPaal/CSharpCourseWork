namespace DAL;

using GameBrain;
using System.Text.Json;
using System.IO;

public class JsonConfigRepository : IConfigRepository
{
    private static IGameRepository gameJsonRespository = new JsonGameRespository();

    public void SaveConfig(string jsonStateString, string gameConfigName)
    {
        File.WriteAllText(FileHelper.BasePath +
                          gameConfigName + "_" +
                          DateTime.Now.ToString("yyyy-MM-dd_HH-mm") +
                          FileHelper.ConfigExtension, jsonStateString);
    }
    public List<string> GetConfigurationNames()
    {
        if (FileHelper.RootFolderGenerator() || FileHelper.DoesRootFolderContainConfigs())
        {
            //generate directory
            GenerateDefaultConfiguration();
            //generate default from default config if none exists
        }

        return Directory
            .GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(
                    Path.GetFileNameWithoutExtension(fullFileName)
                )
            )
            .ToList();
        //load existing configs
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var filePath = FileHelper.BasePath + name + FileHelper.ConfigExtension;
        var configString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<GameConfiguration>(configString)!;
    }

    private void GenerateDefaultConfiguration()
    {
        var defaultConfiguration = new ConfigRepository();
        var defaultConfigurations = defaultConfiguration.GetConfigurationNames();

        foreach (var configName in defaultConfigurations)
        {
            var gameOption = defaultConfiguration.GetConfigurationByName(configName);
            
            var jsonGameOption = JsonSerializer.Serialize(gameOption, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true});

            SaveConfig(jsonGameOption, configName);
        }
    }

    public string CreateGameConfiguration()
    {
        Console.WriteLine("Enter the name of the new configuration:");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Default name";
        }

        Console.WriteLine("Enter the board width (Max 9):");
        string? boardWidthInput = Console.ReadLine();
        int boardWidth = string.IsNullOrWhiteSpace(boardWidthInput) ? 5 : int.Parse(boardWidthInput);

        Console.WriteLine("Enter the board height (Max 9):");
        string? boardHeightInput = Console.ReadLine();
        int boardHeight = string.IsNullOrWhiteSpace(boardHeightInput) ? 5 : int.Parse(boardHeightInput);

        Console.WriteLine("Enter the number of moves after which a piece can be moved:");
        string? movePieceAfterNMovesInput = Console.ReadLine();
        int movePieceAfterNMoves = string.IsNullOrWhiteSpace(movePieceAfterNMovesInput) ? 4 : int.Parse(movePieceAfterNMovesInput);

        Console.WriteLine("Enter the name of the config:");
        Random random = new Random();
        string defaultName = boardWidth.ToString() + "x" + boardHeight.ToString() + random.Next(1, 9999).ToString();
        string? input = Console.ReadLine();
        string configName;
        if (input == null || string.IsNullOrEmpty(input))
        {
            configName = defaultName;
        }
        else
        {
            configName = input;
        }
        
        GameConfiguration gameConfig = new GameConfiguration()
        {
            Name = name,
            BoardSizeWidth = boardWidth,
            BoardSizeHeight = boardHeight,
            MovePieceAfterNMoves = movePieceAfterNMoves,
        };

        var jsonGameOption = JsonSerializer.Serialize(gameConfig, new JsonSerializerOptions { WriteIndented = true });

        SaveConfig(jsonGameOption, configName);
        return "Created";
    }
    
    

}