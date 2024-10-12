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
        
        // main loop of gameplay
        // draw the board again
        // ask input again
        // is game over?

        do
        { 
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            
            Console.WriteLine("Give me a command, DADDY:");
            Console.WriteLine("<1 x,y> to place a piece,");
            Console.WriteLine("<2 x,y> to move the playable area,");
            Console.WriteLine("<3 x,y x,y> to move a placed piece:");
            var input = Console.ReadLine()!;

            var result = RegexValidate(input);
            Console.WriteLine(result.success.ToString());
            if (result.success)
            {
                int instruction = result.output[0, 0];
                int firstX = result.output[1, 0];
                int firstY = result.output[1, 1];

                Console.WriteLine(instruction.ToString() + " " + firstX.ToString() + " " + firstY.ToString());

                if (instruction == 1)
                {
                    if (gameInstance.MakeAMove(firstX, firstY))
                    {
                        if (gameInstance.CheckWinCondition())
                        {
                            ConsoleUI.Visualizer.DrawBoard(gameInstance);
                            Console.WriteLine("Game Over!");
                            break;
                        }
                    }
                } else if (instruction == 2)
                {
                    if (gameInstance.MovePlayableArea(firstX, firstY))
                    {
                        var test = gameInstance._gameArea;
                        Console.WriteLine($"Playable area moved to ({test[0]}, {test[1]})");
                    }
                } else if (instruction == 3)
                {
                    if (result.hasSecondCoords)
                    {
                        int secondX = result.output[2, 0];
                        int secondY = result.output[2, 1];

                        gameInstance.MoveExistingPiece(firstX, firstY, secondX, secondY);


                    }
                }

                if (gameInstance.CheckWinCondition())
                {
                    ConsoleUI.Visualizer.DrawBoard(gameInstance);
                    Console.WriteLine("Game Over!");
                    break;
                }
                
                // Check for tie condition
                if (gameInstance.IsBoardFull())
                {
                    ConsoleUI.Visualizer.DrawBoard(gameInstance);
                    Console.WriteLine("It's a tie!");
                    break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input, you fucking moron. I hope you don't do the same mistake twice!");
            }

        } while (true);
        
        /*// Reset the game or return to menu
        Console.WriteLine("Press 'R' to reset the game or any other key to return to the menu.");
        var key = Console.ReadKey().Key;
        if (key == ConsoleKey.R)
        {
            gameInstance.ResetGame();
            return MainLoop();
        }*/
        
        return "Game Over";
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
        else
        {
            return (null, false, false);
        }

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