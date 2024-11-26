using MenuSystem;

namespace ConsoleApp;

public class Menus
{

    public static readonly Menu DeepMenu = new Menu(
        EMenuLevel.Deep,
        "TIC-TAC-TOE DEEP", [
            new MenuItem()
            {
                Shortcut = "N",
                Title = "NOPE",
            }
        ]
    );
    
    public static readonly Menu OptionsMenu =
        new Menu(
            EMenuLevel.Secondary,
            "TIC-TAC-TWO Options", [
                new MenuItem()
                {
                    Shortcut = "X",
                    Title = "X Starts",
                    MenuItemAction = DeepMenu.Run
                },
                new MenuItem()
                {
                    Shortcut = "O",
                    Title = "O Starts",
                    MenuItemAction = DummyMethod
                }
            
            ]);
    
    public Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", [
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            },
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New Game",
                MenuItemAction = GameController.MainLoop  // Remove () at the end of MainLoop for it to work.
            },
            new MenuItem()
            {
                Shortcut = "L",
                Title = "Load game",
                MenuItemAction = SavedGamesLoader.MainLoop
            }
        ]);
    
    private static string DummyMethod()
    {
        Console.Write("Just press any key to get out from here! (Any key - as a random choice from keyboard....)");
        Console.ReadKey();
        return "foobar";
    }
}