namespace DAL;

using System.IO;

public static class FileHelper
{
    public static string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                                    Path.DirectorySeparatorChar +
                                    "tic-tac-two" +
                                    Path.DirectorySeparatorChar;

    public static string ConfigExtension = ".config.json";
    public static string GameExtension = ".game.json";


    public static bool RootFolderGenerator()
    {
        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
            return true;
        }

        return false;
    }

    public static bool DoesRootFolderContainConfigs()
    {
        if (Directory.Exists(BasePath))
        {
            if (Directory.GetFiles(BasePath, "*" + ConfigExtension).ToList().Count == 0)
            {
                return false;
            }
        }

        return true;
    }

    
}