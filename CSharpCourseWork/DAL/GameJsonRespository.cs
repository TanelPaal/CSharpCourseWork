namespace DAL;

using System.Text.Json;

public class GameJsonRespository: IGameRepository
{
    public void SaveGame(string jsonStateString, string gameConfigName)
    {
        File.WriteAllText(FileHelper.BasePath +
                          gameConfigName + "_" +
                          DateTime.Now.ToString("yyyy-MM-dd_HH-mm") +
                          FileHelper.GameExtension, jsonStateString);


    }
}