using GameBrain;
using System.Text.Json;
using DAL.DB;
using Domain;

namespace DAL;

public class DbConfigRepository : IConfigRepository
{
    private readonly AppDbContextFactory _contextFactory;

    public DbConfigRepository(AppDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public List<string> GetConfigurationNames()
    {
        CreateInitialConfig();

        using var context = _contextFactory.CreateDbContext();
        return context.GameConfigurations.Select(config => config.Name).ToList();
    }

    public object GetConfigurationList()
    {
        CreateInitialConfig();

        using var context = _contextFactory.CreateDbContext();
        return context.GameConfigurations.ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        using var context = _contextFactory.CreateDbContext();
        var config = context.GameConfigurations.FirstOrDefault(config => config.Name == name)!;

        

        GameConfiguration gameConfig = new GameConfiguration(
            config.Name,
            config.BoardSizeWidth,
            config.BoardSizeHeight,
            config.MovePieceAfterNMoves,
            config.PieceLimit
        );
        gameConfig.Id = config.Id;
        
        
        return gameConfig;
    }

    private void CreateInitialConfig()
    {
        using var context = _contextFactory.CreateDbContext();
        if (context.GameConfigurations.Any()) return;

        var hardcodedRepo = new ConfigRepository();
        var optionNames = hardcodedRepo.GetConfigurationNames();

        foreach (var config in optionNames.Select(optionName => hardcodedRepo.GetConfigurationByName(optionName)))
        {

            Configuration gameDBConfig = new Configuration(
                config.Name,
                config.BoardSizeWidth,
                config.BoardSizeHeight,
                config.MovePieceAfterNMoves,
                config.PieceLimit
            );
            gameDBConfig.Id = config.Id;
            
            context.GameConfigurations.Add(gameDBConfig);
        }

        context.SaveChanges();
    }

    public string CreateGameConfiguration()
    {
        Console.WriteLine("Enter the name of the new configuration:");
        string? input = Console.ReadLine();
        
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

        int pieceLimit;
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

        Random random = new Random();
        string defaultName = boardWidth + "x" + boardHeight + random.Next(1, 9999);
        string configName;
        if (input == null || string.IsNullOrEmpty(input))
        {
            configName = defaultName;
        }
        else
        {
            configName = input;
        }
        
        Configuration gameConfig = new Configuration(
        
            configName,
            boardWidth,
            boardHeight,
            movePieceAfterNMoves,
            pieceLimit
        );
        
        using var context = _contextFactory.CreateDbContext();

        context.GameConfigurations.Add(gameConfig);
        context.SaveChanges();

        return "Created";
    }
}