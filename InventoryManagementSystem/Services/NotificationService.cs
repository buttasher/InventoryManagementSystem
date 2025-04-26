using InventoryManagementSystem.Hubs;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public class NotificationService
    {
        private readonly InventoryManagementSystemContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(InventoryManagementSystemContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task SendNotification(string message, int userId, string role)
        {
            // 1. Save the notification to the database
            var notification = new Notification
            {
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false,
                UserId = userId,
                Role = role
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // 2. Push the notification to clients based on the role (e.g., "Admin")
            await _hubContext.Clients.Group(role).SendAsync("ReceiveNotification", message);
        }
    }
}
