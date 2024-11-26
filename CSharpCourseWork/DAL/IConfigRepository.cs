using GameBrain;

namespace DAL;

public interface IConfigRepository
{
    public List<string> GetConfigurationNames();
    public GameConfiguration GetConfigurationByName(string name);
    public string CreateGameConfiguration();
}