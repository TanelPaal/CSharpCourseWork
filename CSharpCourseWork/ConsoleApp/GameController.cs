using System.Text.RegularExpressions;
using DAL;
using DAL.DB;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static readonly AppDbContextFactory ContextFactory = new();
    private static IConfigRepository _configRepository = new DbConfigRepository(contextFactory: ContextFactory);
    private static IGameRepository _gameRepository = new DbGameRepository(contextFactory: ContextFactory);

    // DB <=> JSON

    // private static IConfigRepository _configRepository = new JsonConfigRepository();
    // private static IGameRepository _gameRepository = new JsonGameRespository();
    
    public static string PlayGame(GameState saveGame)
    {
        var gameInstance = new TicTacTwoBrain(saveGame);
        do
        {
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            var input = GetUserInput();

            if (input.Equals("save", StringComparison.InvariantCultureIgnoreCase))
            {
                _gameRepository.SaveGame(
                    gameInstance.GetGameState(),
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
    
    public static string MainLoop()
    {
        GameState saveGame;
        
        var saveOrNew = ChooseSaveOrNew();
    
        if (saveOrNew == "R" || saveOrNew == "E")
        {
            return saveOrNew;
        }

        if (saveOrNew == "NewGame")
        {
            GameConfiguration chosenConfig;

            string chosenConfigShortcut = ChooseConfiguration();
            if (!int.TryParse(chosenConfigShortcut, out int configNo))
            {
                return chosenConfigShortcut;
            }

            chosenConfig = _configRepository.GetConfigurationByName(
                _configRepository.GetConfigurationNames()[configNo]
            );
            
            // Limit the board size to 9x9
            int boardWidth = Math.Min(chosenConfig.BoardSizeWidth, 9);
            int boardHeight = Math.Min(chosenConfig.BoardSizeHeight, 9);
            
            var gameBoard = new EGamePiece[boardWidth][];
            for (var x = 0; x < gameBoard.Length; x++)
            {
                gameBoard[x] = new EGamePiece[boardHeight];
            }

            // Calculate the center position
            int centerX = (int)Math.Floor((double)boardWidth / 2);
            int centerY = (int)Math.Floor((double)boardHeight / 2);
            int[] _gameArea = { centerX, centerY };
            Console.WriteLine($"{centerX}, {centerY}");

            saveGame = new GameState(
                gameBoard,
                chosenConfig,
                _gameArea,
                EGamePiece.X,
                0,
                0
            );

        }
        else
        {
            string saveName = ChooseSave();
            if (saveName == "R" || saveName == "E")
            {
                return saveName;
            }
            //Console.WriteLine(saveName);
            saveGame = _gameRepository.GetSaveByName(saveName);

        }

        PlayGame(saveGame);
        return "";
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
        Console.WriteLine("E to exit the game or type 'save' to save game:");
        return Console.ReadLine()!;
    }

    public static string ProcessInput((int[,] output, bool success, bool hasSecondCoords) result,
        TicTacTwoBrain gameInstance)
    {
        if (!result.success)
        {
            Console.WriteLine("Invalid input.");
            return "Invalid input.";
        }
        
        int instruction = result.output[0, 0];
        
        if (instruction == 'E') // Exit Method
        {

            Console.WriteLine("Exiting the game...");
            Environment.Exit(0);
            return "success";
        }
        
        int firstX = result.output[1, 0];
        int firstY = result.output[1, 1];

        if (instruction == 1)
        {
            if (gameInstance.MakeAMove(firstX, firstY) && gameInstance.CheckWinCondition())
            {
                ConsoleUI.Visualizer.DrawBoard(gameInstance);
                Console.WriteLine("Game Over!");
                return "success";
            }
            else
            {
                return "Error";
            }
        }
        else if (instruction == 2)
        {
            if (gameInstance.MovePlayableArea(firstX, firstY))
            {
                var movedGrid = gameInstance._gameState._gameArea;
                Console.WriteLine($"Playable area moved to ({movedGrid[0]}, {movedGrid[1]})");
                return "success";
            }
            else
            {
                return "Failed to move the playable area";
            }
        }
        else if (instruction == 3 && result.hasSecondCoords)
        {
            int secondX = result.output[2, 0];
            int secondY = result.output[2, 1];
            if (gameInstance.MoveExistingPiece(firstX, firstY, secondX, secondY))
            {
                return "success";
            }
            else
            {
                return "Failed to move the piece";
            }
        }

        return "fuck knows";
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
            "Tic-Tac-Two - New or previous",
            configMenuItems,
            isCustomMenu: true
        );
        
        return configMenu.Run();
    }
}