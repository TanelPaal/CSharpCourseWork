using System.Text.RegularExpressions;
using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static ConfigRepository _configRepository = new ConfigRepository();
    
    public static string MainLoop()
    {
        var chosenConfigShortcut = ChooseConfiguration();
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }

        var chosenConfig = _configRepository.GetConfigurationByName(
            _configRepository.GetConfigurationNames()[configNo]
        );
    
        var gameInstance = new TicTacTwoBrain(chosenConfig);
        
        do
        { 
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            var input = GetUserInput();

            var result = RegexValidate(input);
            if (result.success)
            {
                ProcessInput(result, gameInstance);
                if (gameInstance.CheckWinCondition() || gameInstance.IsBoardFull())
                {
                    ConsoleUI.Visualizer.DrawBoard(gameInstance);
                    Console.WriteLine("Game Over!");
                    break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input, you fucking moron. I hope you don't do the same mistake twice!");
            }

        } while (true);

        return "Game Over";
    }

    private static string GetUserInput()
    {
        Console.WriteLine("Give me a command, DADDY:");
        Console.WriteLine("<1 x,y> to place a piece,");
        Console.WriteLine("<2 x,y> to move the playable area,");
        Console.WriteLine("<3 newX,newY oldX,oldY> to move a placed piece:");
        return Console.ReadLine()!;
    }

    private static void ProcessInput((int[,] output, bool success, bool hasSecondCoords) result,
        TicTacTwoBrain gameInstance)
    {
        int instruction = result.output[0, 0];
        int firstX = result.output[1, 0];
        int firstY = result.output[1, 1];

        if (instruction == 1)
        {
            if (gameInstance.MakeAMove(firstX, firstY) && gameInstance.CheckWinCondition())
            {
                ConsoleUI.Visualizer.DrawBoard(gameInstance);
                Console.WriteLine("Game Over!");
            }
        }
        else if (instruction == 2)
        {
            if (gameInstance.MovePlayableArea(firstX, firstY))
            {
                var movedGrid = gameInstance._gameArea;
                Console.WriteLine($"Playable area moved to ({movedGrid[0]}, {movedGrid[1]})");
            }
        }
        else if (instruction == 3 && result.hasSecondCoords)
        {
            int secondX = result.output[2, 0];
            int secondY = result.output[2, 1];
            gameInstance.MoveExistingPiece(firstX, firstY, secondX, secondY);
        }
    }
    
    private static (int[,] output, bool success, bool hasSecondCoords) RegexValidate(string input)
    {

        string pattern = @"^(1|2|3)\s+(\d{1,2}),(\d{1,2})(?:\s+(\d{1,2}),(\d{1,2}))?$";

        // Create a Regex object
        Regex regex = new Regex(pattern);

        // Match the input string against the regex pattern
        Match match = regex.Match(input);
        
        if (match.Success)
        {
            // Extracting the captured groups
            int firstOutput = int.Parse(match.Groups[1].Value); // First number (1, 2, or 3)
            int firstCoordX = int.Parse(match.Groups[2].Value); // First coordinate X
            int firstCoordY = int.Parse(match.Groups[3].Value); // First coordinate Y
            
            int[,] output = new int[4, 2];

            output[0, 0] = firstOutput;
            output[1, 0] = firstCoordX;
            output[1, 1] = firstCoordY;

            if (match.Groups[4].Success && match.Groups[5].Success)
            {
                int secondCoordX = int.Parse(match.Groups[4].Value); // Second coordinate X (optional)
                int secondCoordY = int.Parse(match.Groups[5].Value); // Second coordinate Y (optional)

                output[2, 0] = secondCoordX;
                output[2, 1] = secondCoordY;

                return (output, true, true);
            }
            
            return (output, true, false);
        }
        return (null, false, false);
    }
    
    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (int i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = _configRepository.GetConfigurationNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }
        
        configMenuItems.Add(new MenuItem()
        {
            Title = "New custom game",
            Shortcut = "N",
            MenuItemAction = () => _configRepository.CreateGameConfiguration()
        });

        var configMenu = new Menu(EMenuLevel.Secondary, 
            "Tic-Tac-Two - choose game config",
            configMenuItems,
            isCustomMenu: true
        );

        return configMenu.Run();
    }
}