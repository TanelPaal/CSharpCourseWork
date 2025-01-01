using GameBrain;

namespace DAL;

using System.Text.Json;
using Domain;

public class ConfigRepository
{
    public List<GameConfiguration> _gameConfigurations = new List<GameConfiguration>()
    {
        new GameConfiguration()
        {
            Name = "Classical"
        },
        new GameConfiguration()
        {
            Name = "Big 5x5",
            BoardSizeWidth = 5,
            BoardSizeHeight = 5,
            MovePieceAfterNMoves = 4,
        },
    };

    public object GetConfigurationList()
    {
        return _gameConfigurations.Select(config => config.Name).ToList();
    }


    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations
            .OrderBy(x => x.Name)
            .Select(config => config.Name)
            .ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        return _gameConfigurations.Single(c => c.Name == name);
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

         GameConfiguration gameConfig = new GameConfiguration()
         {
             Name = name,
             BoardSizeWidth = boardWidth,
             BoardSizeHeight = boardHeight,
             MovePieceAfterNMoves = movePieceAfterNMoves,
         };
         
         string json = JsonSerializer.Serialize(gameConfig, new JsonSerializerOptions { WriteIndented = true });
         
         _gameConfigurations.Add(gameConfig);
         return "Created";
     }
}