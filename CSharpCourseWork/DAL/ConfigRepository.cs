using GameBrain;

namespace DAL;

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
    
    // n
    // testing 5,5 4
    
     public string CreateGameConfiguration()
     {
         Console.WriteLine("Enter the name of the new configuration:");
         string? name = Console.ReadLine();

         Console.WriteLine("Enter the board width:");
         int? boardWidth = int.Parse(Console.ReadLine());

         Console.WriteLine("Enter the board height:");
         int? boardHeight = int.Parse(Console.ReadLine());

         Console.WriteLine("Enter the number of moves after which a piece can be moved:");
         int? movePieceAfterNMoves = int.Parse(Console.ReadLine());
         
         
         GameConfiguration gameConfig = new GameConfiguration()
         {
             Name = name ?? "Default name",
             BoardSizeWidth = boardWidth ?? 5,
             BoardSizeHeight = boardHeight ?? 5,
             MovePieceAfterNMoves = movePieceAfterNMoves ?? 4,
             
         };
         _gameConfigurations.Add(gameConfig);
         return "Created";
     }
}