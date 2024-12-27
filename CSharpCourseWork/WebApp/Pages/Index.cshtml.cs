using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    public string ErrorMessage { get; private set; } = "";

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
                else
                {
                    CookieOptions options = new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(7), // Expires in 7 days
                        HttpOnly = true, // Prevents JavaScript from accessing the cookie
                        Secure = Request.IsHttps // Set to true if using HTTPS
                    };
                    Response.Cookies.Append("Username", username, options);
                    return RedirectToPage("./Dashboard");
                }
            default:
                return Page();
        }
    }

}