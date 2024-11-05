using DAL;
using MenuSystem;

namespace ConsoleApp;

public static class SavedGamesLoader
{
    static IGameRepository gameRespository = new JsonGameRespository();

    static string ChooseGame()
    {
        Console.Clear();
        var savedGamesMenuItems = new List<MenuItem>();
        var allSavedGames = gameRespository.GetSaveNames();

        for (var i = 0; i < allSavedGames.Count; i++)
        {
            var shortcut = (i + 1).ToString();
            savedGamesMenuItems.Add(
                new MenuItem
                {
                    Title = allSavedGames[i],
                    Shortcut = shortcut,
                    MenuItemAction = () => shortcut
                });
        }

        var savedGamesMenu = new Menu(EMenuLevel.Secondary, "Choose a game to continue", savedGamesMenuItems);
        return savedGamesMenu.Run();
    }
    
    public static string MainLoop()
    {
        Console.Clear();
        string gameSaveId = ChooseGame();
        Console.WriteLine("Loading Game Save");

        var allSavedGames = gameRespository.GetSaveNames();

        for (var i = 0; i < allSavedGames.Count; i++)
        {
            if ((i + 1).ToString() == gameSaveId)
            {
                var gameSaveName = allSavedGames[i];
                var gameSave = gameRespository.GetSaveByName(gameSaveName);

                Console.WriteLine("Loaded Game Save");
                GameController.PlayGame(gameSave);
            }
        }
        
        return "";

    }
 
}