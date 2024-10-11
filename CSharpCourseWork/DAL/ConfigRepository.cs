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
            WinCondition = 4, // not used anywhere else besides here
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
}