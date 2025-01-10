using MenuSystem;

var optionsMenu = new Menu(
    EMenuLevel.Secondary,
    "TIC-TAC-TOE Options", [
    new MenuItem()
    {
        Shortcut = "X",
        Title = "X Starts",
        MenuItemAction = null
    },
    new MenuItem()
    {
    Shortcut = "O",
    Title = "O Starts",
    MenuItemAction = null
    },

]);

var mainMenu = new Menu(
    EMenuLevel.Main,
    "TIC-TAC-TOE", [
    new MenuItem()
    {
        Shortcut = "O",
        Title = "Options",
        MenuItemAction = optionsMenu.Run
    },
    new MenuItem()
    {
        Shortcut = "N",
        Title = "New game",
        MenuItemAction = null
    }
]);

mainMenu.Run();

return;
// ========================================
