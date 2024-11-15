using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageClient.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AuthService.AuthServiceClient _authClient;
        private readonly ItemService.ItemServiceClient _itemClient;

        public IndexModel(AuthService.AuthServiceClient authClient,
            ItemService.ItemServiceClient itemClient)
        {
            _authClient = authClient;
            _itemClient = itemClient;

        }

        public string LoginMessage { get; set; }

        // Method to handle login via gRPC
        public async Task<IActionResult> OnPostLoginAsync(string username, string password)
        {
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response = await _authClient.LoginAsync(loginRequest);
            if (response != null)
            {
                // Store the token in session
                HttpContext.Session.SetString("JwtToken", response.Token);
                LoginMessage = "Login successful! Token stored in session.";
                return RedirectToPage("/ItemCRUD");
            }
            else
            {
                LoginMessage = "Login failed!";
            }

            return Page();
        }
    }
}
