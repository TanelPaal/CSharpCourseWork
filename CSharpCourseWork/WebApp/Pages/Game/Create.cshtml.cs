using System.ComponentModel.DataAnnotations;
using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Game;

public class Create : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public Create(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    [BindProperty] 
    public List<string> Configurations { get; set; } = new List<string>();

    [BindProperty]
    public string SelectedConfiguration { get; set; } = "";

    [BindProperty]
    public string GameName { get; set; } = string.Empty;

    
    [BindProperty]
    public string PlayerOneType { get; set; } = "Self";

    [BindProperty]
    public string PlayerTwoType { get; set; } = "Self";

    public void OnGet()
    {
        // You can load data from a database or other sources here
        Configurations = _configRepository.GetConfigurationNames();
    }



    public IActionResult OnPost(string SelectedConfiguration)
    {




        Configurations = _configRepository.GetConfigurationNames();


        if (string.IsNullOrEmpty(SelectedConfiguration))
        {
            ModelState.AddModelError(string.Empty, "Please select a configuration.");
            return Page();
        }

        var config = _configRepository.GetConfigurationByName(SelectedConfiguration);
        
        var gameState = new GameState(
            new EGamePiece[config.BoardSizeWidth][], // Initialize the game board with the correct size
            config,
            new int[config.BoardSizeWidth * config.BoardSizeHeight], // Initialize the game area
            EGamePiece.X, // Default next move by X
            0, // Initial X turn count
            0  // Initial O turn count
        );
        
        _gameRepository.SaveGame(gameState, "NewGame");

        return Page();
    }
    
    
    


    public void HandleItemClick(string item)
    {
        SelectedConfiguration = item;
    }
    

}
