using System.ComponentModel.DataAnnotations;
using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain;

namespace WebApp.Pages.Game;

public class Create : PageModel
{
    private IConfigRepository _configRepository;
    private IGameRepository _gameRepository;

    public Create(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    [BindProperty] 
    public List<Configuration> Configurations { get; set; } = new List<Configuration>();

    [BindProperty]
    public string SelectedConfiguration { get; set; } = "";

    [BindProperty]
    public string GameName { get; set; } = string.Empty;

    // Note: The player types are not used in this version of the game.
    /*[BindProperty]
    public string PlayerOneType { get; set; } = "Self";

    [BindProperty]
    public string PlayerTwoType { get; set; } = "Self";*/

    public void OnGet()
    {
        // You can load data from a database or other sources here
        Configurations = (List<Configuration>)_configRepository.GetConfigurationList();
    }


    public IActionResult OnPost(string SelectedConfiguration)
    {
        Configurations = (List<Configuration>)_configRepository.GetConfigurationList();
        
        if (string.IsNullOrEmpty(SelectedConfiguration))
        {
            ModelState.AddModelError(string.Empty, "Please select a configuration.");
            return Page();
        }
        
        if (string.IsNullOrEmpty(GameName))
        {
            ModelState.AddModelError(string.Empty, "Please enter a game name.");
            return Page();
        }

        var config = _configRepository.GetConfigurationByName(SelectedConfiguration);
        
        // Initialize the game board with the correct size
        var gameBoard = new EGamePiece[config.BoardSizeWidth][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[config.BoardSizeHeight];
        }

        // Initialize the game area
        int[] gameArea = new int[config.BoardSizeWidth * config.BoardSizeHeight];

        var gameState = new GameState(
            gameBoard,
            config,
            gameArea,
            EGamePiece.X, // Default next move by X
            0, // Initial X turn count
            0  // Initial O turn count
        );
        
        
        _gameRepository.SaveGame(gameState, GameName);

        // Get username from query parameter
        var username = HttpContext.Request.Query["username"].ToString();
        return RedirectToPage("/Dashboard", new { username = username });
    }
    

    public void HandleItemClick(string item)
    {
        SelectedConfiguration = item;
    }
}
