namespace DAL;
using GameBrain;
public interface IGameRepository
{
    public void SaveGame(GameState gameState, string gameSaveName);
    public GameState GetSaveByName(string gameSaveName);
    public List<string> GetSaveNames();
    public List<GameState> GetAllSavedGames();
    public GameState GetSaveById(int gameId);
}