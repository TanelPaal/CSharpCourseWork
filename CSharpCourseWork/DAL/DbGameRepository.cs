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

    public void SaveGame(GameState gameState, string gameSaveName = null!)
    {
        using var context = _contextFactory.CreateDbContext();

        // Check if a saved game with the same name exists
        var existingSavedGame = context.SavedGames.FirstOrDefault(g => g.Name == gameSaveName);
    
        if (existingSavedGame != null)
        {
            // Delete the existing saved game
            context.SavedGames.Remove(existingSavedGame);
        }

        // Create a new saved game
        SavedGame dtoSavedGame = new SavedGame
        {
            Id = gameState.Id,
            Name = gameSaveName,
            State = gameState.ToString(),
            ConfigurationId = gameState.GameConfiguration.Id,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        context.SavedGames.Add(dtoSavedGame);
        context.SaveChanges();

        /*if (gameInDb == null)
        {
            if (string.IsNullOrEmpty(gameSaveName))
            {
                gameSaveName = $"{gameState.GameConfiguration.Name}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            }
            
            SavedGame dtoSavedGame = new SavedGame
            {
                Id = gameState.Id,
                Name = gameSaveName,
                State = gameState.ToString(),
                ConfigurationId = gameState.GameConfiguration.Id
            };
            Console.WriteLine(gameState.GameConfiguration.Id);
            Console.WriteLine(gameState.Id);
            Console.WriteLine(dtoSavedGame.ConfigurationId);
            
            GameConfiguration DTOConfig = gameState.GameConfiguration;
            Configuration tempConfiguration = new Configuration(
                DTOConfig.Name,
                DTOConfig.BoardSizeWidth,
                DTOConfig.BoardSizeHeight,
                DTOConfig.MovePieceAfterNMoves, 
                DTOConfig.PieceLimit
            );
            tempConfiguration.Id = gameState.GameConfiguration.Id;

            var configExists = context.GameConfigurations.FirstOrDefault(g => g.Id == gameState.GameConfiguration.Id)!;
            Console.WriteLine(configExists);

            if (configExists == null)
            {

                context.GameConfigurations.Add(tempConfiguration);
                context.SaveChanges();

                dtoSavedGame.ConfigurationId = tempConfiguration.Id; // Update the foreign key
            }

            context.SavedGames.Add(dtoSavedGame);
        }
        else
        {
            gameInDb.State = gameState.ToString();
            gameInDb.ModifiedAt = DateTime.UtcNow;
        }

        context.SaveChanges();*/
    }

    public GameState GetSaveByName(string gameSaveName)
    {
        using var context = _contextFactory.CreateDbContext();
        var savedGame = context.SavedGames.FirstOrDefault(g => g.Name == gameSaveName)!;
        

        var gameData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(savedGame.State!)!;
        
        var gameConfiguration = context.GameConfigurations.FirstOrDefault(g => g.Id == savedGame.ConfigurationId)!;

        
        GameConfiguration DTOConfig = new GameConfiguration(
            gameConfiguration.Name,
            gameConfiguration.BoardSizeWidth,
            gameConfiguration.BoardSizeHeight,
            gameConfiguration.MovePieceAfterNMoves, 
            gameConfiguration.PieceLimit
        );
        DTOConfig.Id = gameConfiguration.Id;
        
        var gameArea = JsonSerializer.Deserialize<int[]>(((JsonElement)gameData["_gameArea"]).GetRawText());

        
        var gameState = new GameState(
             // Extract GameBoard and cast to EGamePiece[][]
            (gameData["GameBoard"]).Deserialize<EGamePiece[][]>(),
            // Extract GameConfiguration and cast to GameConfiguration object
            DTOConfig,
            // Extract _gameArea and cast to int[]
            gameArea,
            // Extract _nextMoveBy and cast to EGamePiece
            (EGamePiece)gameData["_nextMoveBy"].GetInt32(),
            // Extract _xTurnCount and cast to int
            gameData["_xTurnCount"].GetInt32(),
            // Extract _oTurnCount and cast to int
            gameData["_oTurnCount"].GetInt32()
        );
        Console.WriteLine(gameState.Id);
        gameState.Id = gameData["Id"].GetInt32();
        Console.WriteLine(gameState.Id);
        Console.WriteLine(gameData["Id"].GetInt32());
        
        return gameState;

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