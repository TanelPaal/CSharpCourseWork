using System.Text.Json;
using DAL.DB;
using Domain;
using GameBrain;

namespace DAL;

public class DbGameRepository : IGameRepository
{
    private readonly AppDbContextFactory _contextFactory;

    public DbGameRepository(AppDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void SaveGame(GameState gameState, string gameSaveName)
    {
        using var context = _contextFactory.CreateDbContext();

        var gameInDb = context.SavedGames.FirstOrDefault(g => g.Id == gameState.Id);

        if (gameInDb == null)
        {
            context.Attach(gameState.GameConfiguration);
            context.Add(gameState);
        }
        else
        {
            gameInDb.State = gameState.ToString();
            gameInDb.ModifiedAt = DateTime.UtcNow;
        }

        context.SaveChanges();
    }

    public GameState GetSaveByName(string gameSaveName)
    {
        using var context = _contextFactory.CreateDbContext();
        var savedGame = context.SavedGames.FirstOrDefault(g => g.Name == gameSaveName)!;

        return JsonSerializer.Deserialize<GameState>(savedGame.State)!;

    }

    public List<string> GetSaveNames()
    {
        using var context = _contextFactory.CreateDbContext();
        List<string> gameNames = new List<string>();

        foreach (var configId in context.SavedGames.Select(g => g).ToList())
        {
            gameNames.Add(configId.Name);
        }



        return gameNames;
    }

    public SavedGame? GetGameById(int gameId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.SavedGames.FirstOrDefault(g => g.Id == gameId);
    }
}



/*
 
 ganecontroller should make a savegame from current game then transfer the savegame to this class.
 We then save that to db
 
 if we ever need to load a savegame from db we return a savedGame to the gamecontroller and it will make a new gamestate from the savedGame
        var game = new SavedGame
        {
            Configuration = config,
            State = gameInstance.GetGameBoardStateJson(),
            PlayerXName = playerXName,
            PlayerYName = playerYName
        };
        
        */