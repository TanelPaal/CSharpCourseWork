using System.Text.RegularExpressions;
using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static IConfigRepository _configRepository = new ConfigJsonRepository();
    private static IGameRepository _gameRepository = new GameJsonRespository();
    
    
    public static string MainLoop()
    {


        GameConfiguration chosenConfig;
        TicTacTwoBrain gameInstance;
        if (ChooseSaveOrNew() == "NewGame")
        {
            string chosenConfigShortcut = ChooseConfiguration();
            if (!int.TryParse(chosenConfigShortcut, out int configNo))
            {
                return chosenConfigShortcut;
            }
            
            chosenConfig = _configRepository.GetConfigurationByName(
                _configRepository.GetConfigurationNames()[configNo]
            );
            gameInstance = new TicTacTwoBrain(chosenConfig);
        }
        else
        {
            string saveName = ChooseSave();
            Console.WriteLine(saveName);
            GameState saveGame = _gameRepository.GetSaveByName(saveName);

            gameInstance = new TicTacTwoBrain(saveGame);
        }
        
        do
        { 
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            var input = GetUserInput();

            if (input.Equals("save", StringComparison.InvariantCultureIgnoreCase))
            {
                _gameRepository.SaveGame(
                    gameInstance.GetGameStateJson(),
                    gameInstance.GetGameConfigName()
                );
                break;
            }

            var result = RegexValidate(input);
            if (result.success)
            {
                ProcessInput(result, gameInstance);
                if (IsGameOver(gameInstance))
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
    
    private static bool IsGameOver(TicTacTwoBrain gameInstance)
    {
        return gameInstance.CheckWinCondition() || gameInstance.IsBoardFull();
    }

    private static string GetUserInput()
    {
        Console.WriteLine("Give me a command, DADDY:");
        Console.WriteLine("<1 x,y> to place a piece,");
        Console.WriteLine("<2 x,y> to move the playable area,");
        Console.WriteLine("<3 newX,newY oldX,oldY> to move a placed piece:");
        Console.WriteLine("E to exit the game:");
        return Console.ReadLine()!;
    }

    private static void ProcessInput((int[,] output, bool success, bool hasSecondCoords) result,
        TicTacTwoBrain gameInstance)
    {
        
        if (!result.success)
        {
            Console.WriteLine("Invalid input.");
            return;
        }
        
        int instruction = result.output[0, 0];
        
        if (instruction == 'E') // Exit Method
        {
            Console.WriteLine("Exiting the game...");
            Environment.Exit(0);
        }
        
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
                var movedGrid = gameInstance._gameState._gameArea;
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
        if (input.Trim().ToUpper() == "E")
        {
            return (new int[1, 1] { { 'E' } }, true, false);
        }


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
    
    private static string ChooseSave()
    {
        var configMenuItems = new List<MenuItem>();
        for (int i = 0; i < _gameRepository.GetSaveNames().Count; i++)
        {
            var returnValue = _gameRepository.GetSaveNames()[i];
            configMenuItems.Add(new MenuItem()
            {
                Title = _gameRepository.GetSaveNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }
        

        var configMenu = new Menu(EMenuLevel.Secondary, 
            "Tic-Tac-Two - choose game save",
            configMenuItems,
            isCustomMenu: true
        );
        
        

        return configMenu.Run();
    }
    
    private static string ChooseSaveOrNew()
    {
        var configMenuItems = new List<MenuItem>();
        configMenuItems.Add(new MenuItem()
        {
            Title = "New game",
            Shortcut = "N",
            MenuItemAction = () => "NewGame",
        });
        
        configMenuItems.Add(new MenuItem()
        {
            Title = "Load previous game",
            Shortcut = "L",
            MenuItemAction = () => "LoadGame",
        });
        

        var configMenu = new Menu(EMenuLevel.Secondary, 
            "Tic-Tac-Two - New or previo",
            configMenuItems,
            isCustomMenu: true
        );
        
        

        return configMenu.Run();
    }
}