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
        var existingSavedGame = context.SavedGames.FirstOrDefault(g => g.Id == gameState.Id);

        Console.WriteLine(gameState.Id);
        if (existingSavedGame != null)
        {
            // Update existing game
            existingSavedGame.State = JsonSerializer.Serialize(gameState);
            existingSavedGame.ModifiedAt = DateTime.UtcNow;
            Console.WriteLine("updated state");
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("generating new savegame");
            // Create new game
            var dtoSavedGame = new SavedGame
            {
                Id = gameState.Id, // Use the unique ID we generated
                Name = gameSaveName,
                State = JsonSerializer.Serialize(gameState),
                ConfigurationId = gameState.GameConfiguration.Id,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
        
            context.SavedGames.Add(dtoSavedGame);
            context.SaveChanges();
        }
    }


    public GameState GetSaveById(int gameId)
    {
        using var context = _contextFactory.CreateDbContext();
        var saveGameName = context.SavedGames.FirstOrDefault(g => g.Id == gameId)!.Name;
        var saveGame = GetSaveByName(saveGameName);
        return saveGame;
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
        gameState.Id = savedGame.Id;
        Console.WriteLine(gameState.Id);
        
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

    /*public SavedGame? GetGameById(int gameId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.SavedGames.FirstOrDefault(g => g.Id == gameId);
    }*/
    
    public List<GameState> GetAllSavedGames()
    {
        using var context = _contextFactory.CreateDbContext();
        var savedGames = context.SavedGames.ToList();
        var gameStates = new List<GameState>();
    
        foreach (var savedGame in savedGames)
        {
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
                (gameData["GameBoard"]).Deserialize<EGamePiece[][]>(),
                DTOConfig,
                gameArea,
                (EGamePiece)gameData["_nextMoveBy"].GetInt32(),
                gameData["_xTurnCount"].GetInt32(),
                gameData["_oTurnCount"].GetInt32()
            );
            gameState.Id = savedGame.Id;
            gameStates.Add(gameState);
        }
    
        return gameStates;
    }
}