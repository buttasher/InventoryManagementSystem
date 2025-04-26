using Microsoft.AspNetCore.SignalR;

namespace InventoryManagementSystem.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task OnConnectedAsync()
        {
            var role = Context.GetHttpContext()?.Request?.Cookies["role"]; // Example: get role from cookies or context
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, role); // Add user to role group
            }

            await base.OnConnectedAsync();
        }
    }
}
