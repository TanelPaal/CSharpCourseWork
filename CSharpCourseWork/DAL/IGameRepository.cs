namespace DAL;
using GameBrain;
public interface IGameRepository
{
    public void SaveGame(string jsonStateString, string gameSaveName);
    public void SaveConfig(string jsonStateString, string gameConfigName);
    
    public GameState GetSaveByName(string gameSaveName);
    public bool DoesRootFolderContainSaves();
    public List<string> GetSaveNames();




}