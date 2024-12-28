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
                          gameConfigName + "(" +
                          DateTime.Now.ToString("yyyyMMdd_HHmm") + ")" +
                          FileHelper.ConfigExtension, jsonStateString);
    }


    public object GetConfigurationList()
    {
        var ConfigNames = GetConfigurationNames();
        List<GameConfiguration> ConfigList = new List<GameConfiguration>();
        foreach (var configName in ConfigNames)
        {
            var config = GetConfigurationByName(configName);
            ConfigList.Append(config);
        }

        return ConfigList;
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

        int boardWidth;
        while (true)
        {
            Console.WriteLine("Enter the board width (Max 9):");
            string? boardWidthInput = Console.ReadLine();
            if (int.TryParse(boardWidthInput, out boardWidth) && boardWidth <= 9)
            {
                break;
            }
            Console.WriteLine("Invalid input. Please enter a number between 1 and 9.");
        }

        int boardHeight;
        while (true)
        {
            Console.WriteLine("Enter the board height (Max 9):");
            string? boardHeightInput = Console.ReadLine();
            if (int.TryParse(boardHeightInput, out boardHeight) && boardHeight <= 9)
            {
                break;
            }
            Console.WriteLine("Invalid input. Please enter a number between 1 and 9.");
        }

        int movePieceAfterNMoves;
        while (true)
        {
            Console.WriteLine("Enter the number of moves after which a piece can be moved:");
            string? movePieceAfterNMovesInput = Console.ReadLine();
            if (int.TryParse(movePieceAfterNMovesInput, out movePieceAfterNMoves))
            {
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }

        int pieceLimit; // Not Implemented in DB or JSON
        while (true)
        {
            Console.WriteLine("Enter the piece limit:");
            string? pieceLimitInput = Console.ReadLine();
            if (int.TryParse(pieceLimitInput, out pieceLimit))
            {
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }

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