using MenuSystem;
using GameBrain;

var gameInstance = new TicTacTwoBrain(5);

var deepMenu = new Menu(
    EMenuLevel.Deep,
    "TIC-TAC-TWO DEEP", [
        new MenuItem()
        {
            Shortcut = "Y",
            Title = "YYYYYYY",
            MenuItemAction = DummyMethod
        },
    ]);

var optionsMenu = new Menu(
    EMenuLevel.Secondary,
    "TIC-TAC-TWO Options", [
        new MenuItem()
        {
            Shortcut = "X",
            Title = "X Starts",
            MenuItemAction = deepMenu.Run
        },
        new MenuItem()
        {
            Shortcut = "O",
            Title = "O Starts",
            MenuItemAction = DummyMethod
        }
        
    ]);
    
var mainMenu = new Menu(
    EMenuLevel.Main,
    "TIC-TAC-TWO", [
    new MenuItem()
    {
        Shortcut = "O",
        Title = "Options",
        MenuItemAction = optionsMenu.Run
    },
    new MenuItem()
    {
        Shortcut = "N",
        Title = "New Game",
        MenuItemAction = NewGame
    }
]);

mainMenu.Run();

return;
// ========================================

string DummyMethod()
{
    Console.Write("Just press any key to get out from here! (Any key - as a random choice from keyboard....)");
    Console.ReadKey();
    return "foobar";
}

string NewGame()
{
    ConsoleUI.Visualizer.DrawBoard(gameInstance);
    
    Console.Write("Give me coordinates <x,y>:");
    var input = Console.ReadLine()!;
    var inputSplit = input.Split(",");
    var inputX = int.Parse(inputSplit[0]);
    var inputY = int.Parse(inputSplit[1]);
    gameInstance.MakeAMove(inputX, inputY);
    
    // loop
    // draw the board again
    // ask input again
    // is game over?
    
    return "";
}