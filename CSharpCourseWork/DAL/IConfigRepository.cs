using System.Runtime.InteropServices.JavaScript;
using GameBrain;
namespace DAL;
using Domain;

public interface IConfigRepository
{
    public List<string> GetConfigurationNames();
    public object GetConfigurationList();
    public GameConfiguration GetConfigurationByName(string name);
    public string CreateGameConfiguration();
    
}