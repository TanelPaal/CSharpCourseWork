using DAL;
using DAL.DB;
using MenuSystem;

namespace ConsoleApp;

public static class SavedGamesLoader
{

    private static readonly AppDbContextFactory ContextFactory = new();
    private static IGameRepository _gameRepository = new DbGameRepository(contextFactory: ContextFactory);

    // DB <=> JSON

    // static IGameRepository _gameRepository = new JsonGameRespository();

    static string ChooseGame()
    {
        Console.Clear();
        var savedGamesMenuItems = new List<MenuItem>();
        var allSavedGames = _gameRepository.GetSaveNames();

        for (var i = 0; i < allSavedGames.Count; i++)
        {
            var shortcut = (i + 1).ToString();
            savedGamesMenuItems.Add(
                new MenuItem
                {
                    Title = allSavedGames[i],
                    Shortcut = shortcut,
                    MenuItemAction = () => shortcut
                }
            );
        }

        if (savedGamesMenuItems.Count == 0)
        {
            savedGamesMenuItems.Add(
                new MenuItem
                {
                    Title = "No games saved",
                    Shortcut = "1",
                    MenuItemAction = () => ""
                }
            );
        }

        var savedGamesMenu = new Menu(EMenuLevel.Secondary, "Choose a game to continue", savedGamesMenuItems);
        return savedGamesMenu.Run();
    }
    
    public static string MainLoop()
    {
        Console.Clear();
        string gameSaveId = ChooseGame();
        Console.WriteLine("Loading Game Save");

        var allSavedGames = _gameRepository.GetSaveNames();

        for (var i = 0; i < allSavedGames.Count; i++)
        {
            if ((i + 1).ToString() == gameSaveId)
            {
                var gameSaveName = allSavedGames[i];
                var gameSave = _gameRepository.GetSaveByName(gameSaveName);

                Console.WriteLine("Loaded Game Save");
                GameController.PlayGame(gameSave);
            }
        }
        
        return "";

    }
 
}