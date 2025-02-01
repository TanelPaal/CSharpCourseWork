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
    
    public Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", [
            new MenuItem()
            {
                Shortcut = "S",
                Title = "Start",
                MenuItemAction = GameController.MainLoop
            }
        ]);
}