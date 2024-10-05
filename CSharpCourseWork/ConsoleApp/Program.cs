using MenuSystem;

var mainMenu = new Menu("TIC-TAC-TWO", [
    new MenuItem()
    {
        Shortcut = "O",
        Title = "Options",
    },
    new MenuItem()
    {
        Shortcut = "N",
        Title = "New Game",
    }
]);

mainMenu.Run();

return;
// ========================================

static void MenuMain()
{
    MenuStart();
    
    Console.WriteLine("O) Options");
    Console.WriteLine("N) New Game");
    Console.WriteLine("L) Load Game");
    Console.WriteLine("Q) Exit");
    
    MenuEnd();
}

void MenuOptions()
{
    MenuStart();
    
    Console.WriteLine("Choose symbol for player one (X)");
    Console.WriteLine("Choose symbol for player one (O)");
    
    MenuEnd();
}

static void MenuEnd()
{
    Console.WriteLine();
    Console.Write(">");
}

static void MenuStart()
{
    Console.Clear();
    Console.WriteLine("TIC-TAC-TWO");
    Console.WriteLine("==================");
}