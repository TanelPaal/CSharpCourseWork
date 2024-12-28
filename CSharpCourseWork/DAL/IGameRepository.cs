namespace DAL;
using GameBrain;
public interface IGameRepository
{
    public void SaveGame(GameState gameState, string gameSaveName);
    public GameState GetSaveByName(string gameSaveName);
    public List<string> GetSaveNames();

    public GameState GetSaveById(int gameId);
}