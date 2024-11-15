using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageClient.Pages
{
    public class ItemCRUDModel : PageModel
    {
        private readonly ItemService.ItemServiceClient _itemClient;

        public ItemCRUDModel(ItemService.ItemServiceClient itemClient)
        {
            _itemClient = itemClient;
        }

        // Properties to bind to the Razor page form
        [BindProperty]
        public Item NewItem { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();

        // This method gets called when the page is loaded to display all items
        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); // If no token is found, redirect to login
            }
            var metadata = new Metadata
            {
                { "Authorization", $"Bearer {token}" }  // Add JWT token in Authorization header
            };

            var request = new ListItemsRequest();
            var response = await _itemClient.ListItemsAsync(request, metadata);
            Items = response.Items.ToList();
            return Page();
        }

        // This method handles creating a new item
        public async Task<IActionResult> OnPostCreateAsync()
        {
            // Get the JWT token from session
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); // If no token is found, redirect to login
            }
            var metadata = new Metadata
            {
                { "Authorization", $"Bearer {token}" }  // Add JWT token in Authorization header
            };

            // Make the gRPC request to create an item
            var createItemRequest = new CreateItemRequest
            {
                Name = NewItem.Name,
                Description = NewItem.Description
            };

            // Assuming your gRPC server has a CreateItem method in the service
            var response = await _itemClient.CreateItemAsync(createItemRequest, metadata);

            return RedirectToPage(); // Redirect back to the same page after item creation
        }

        // This method handles updating an existing item
        public async Task<IActionResult> OnPostUpdateAsync(string id, string name, string description)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); // If no token is found, redirect to login
            }
            var metadata = new Metadata
            {
                { "Authorization", $"Bearer {token}" }  // Add JWT token in Authorization header
            };
            var updateItemRequest = new UpdateItemRequest
            {
                Id = id,
                Name = name,
                Description = description
            };

            var response = await _itemClient.UpdateItemAsync(updateItemRequest, metadata);

            return RedirectToPage();
        }

        // This method handles deleting an item
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); // If no token is found, redirect to login
            }
            var metadata = new Metadata
            {
                { "Authorization", $"Bearer {token}" }  // Add JWT token in Authorization header
            };
            var deleteItemRequest = new DeleteItemRequest
            {
                Id = id
            };

            var response = await _itemClient.DeleteItemAsync(deleteItemRequest, metadata);

            return RedirectToPage();
        }
    }
}
