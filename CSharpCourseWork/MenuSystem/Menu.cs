namespace MenuSystem;

public class Menu
{
    private string MenuHeader { get; set; }
    private static string _menuDivider = "====================";
    private List<MenuItem> MenuItems { get; set; }
    
    public Menu(string menuHeader, List<MenuItem> menuItems)
    {
        if (string.IsNullOrWhiteSpace(menuHeader))
        {
            throw new ApplicationException("Menu header cannot be null or empty");
        }
        
        MenuHeader = menuHeader;
        
        if (menuItems == null || menuItems.Count == 0)
        {
            throw new ApplicationException("Menu items cannot be null or empty");
        }
        
        MenuItems = menuItems;
    }

    public void Run()
    {
        Console.Clear();

        var userInput = "";
        do
        {
            DrawMenu();
            
            userInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("Be nice!!! Choose actually something!!! Try again....Oh GOD...");
                Console.WriteLine();
            }
            else
            {
                userInput = userInput.ToUpper();

                var userInputOk = false;

                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.Shortcut.ToUpper() != userInput) continue;

                    userInputOk = true;
                    break;
                }

                if (userInputOk == false)
                {
                    userInput = "";
                    Console.WriteLine("For the love of God choose something from the existing options...A-10 taking off!");
                    Console.WriteLine();
                }
            }
            
        } while (string.IsNullOrWhiteSpace(userInput));
    }

    private void DrawMenu()
    {
        Console.WriteLine(MenuHeader);
        Console.WriteLine(_menuDivider);
        
        foreach (var t in MenuItems)
        {
            Console.WriteLine(t);
        }

        Console.WriteLine();

        Console.Write(">");
    }
}