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

    public GameConfiguration GetConfigurationByName(string name)
    {
        using var context = _contextFactory.CreateDbContext();
        var config = context.GameConfigurations.FirstOrDefault(config => config.Name == name)!;
        return config;
    }

    private void CreateInitialConfig()
    {
        using var context = _contextFactory.CreateDbContext();
        if (context.GameConfigurations.Any()) return;

        var hardcodedRepo = new ConfigRepository();
        var optionNames = hardcodedRepo.GetConfigurationNames();

        foreach (var option in optionNames.Select(optionName => hardcodedRepo.GetConfigurationByName(optionName)))
        {
            context.GameConfigurations.Add(option);
        }

        context.SaveChanges();
    }

    public string CreateGameConfiguration()
    {
        Console.WriteLine("Enter the name of the new configuration:");
        string? input = Console.ReadLine();

        Console.WriteLine("Enter the board width:");
        string? boardWidthInput = Console.ReadLine();
        int boardWidth = string.IsNullOrWhiteSpace(boardWidthInput) ? 5 : int.Parse(boardWidthInput);

        Console.WriteLine("Enter the board height:");
        string? boardHeightInput = Console.ReadLine();
        int boardHeight = string.IsNullOrWhiteSpace(boardHeightInput) ? 5 : int.Parse(boardHeightInput);

        Console.WriteLine("Enter the number of moves after which a piece can be moved:");
        string? movePieceAfterNMovesInput = Console.ReadLine();
        int movePieceAfterNMoves = string.IsNullOrWhiteSpace(movePieceAfterNMovesInput) ? 4 : int.Parse(movePieceAfterNMovesInput);

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


        GameConfiguration gameConfig = new GameConfiguration()
        {
            Name = configName,
            BoardSizeWidth = boardWidth,
            BoardSizeHeight = boardHeight,
            MovePieceAfterNMoves = movePieceAfterNMoves,
        };


        using var context = _contextFactory.CreateDbContext();

        context.GameConfigurations.Add(gameConfig);
        context.SaveChanges();

        return "Created";
    }
}