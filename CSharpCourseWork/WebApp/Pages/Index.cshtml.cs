using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.UserServ;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly UserService _userService;

    public string ErrorMessage { get; private set; } = "";

    public IndexModel(UserService userService)
    {
        _userService = userService;
    }

    public IActionResult OnPost(string action, string? username)
    {
        
        switch (action)
        {
            case "Login":
                if (string.IsNullOrEmpty(username))
                {
                    ErrorMessage = "Empty username!";
                    return Page();
                }
                else if (_userService.IsUsernameTaken(username))
                {
                    ErrorMessage = "Username is already taken.";
                    return Page();
                }
                else if (!_userService.CanAddUser())
                {
                    ErrorMessage = "Maximum number of players reached.";
                    return Page();
                }
                else
                {
                    _userService.AddUser(username);

                    HttpContext.Session.SetString("Username", username);
                    return Redirect($"/Dashboard?username={username}");
                }
            default:
                return Page();
        }
    }

}