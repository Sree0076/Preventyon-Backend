using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Repository.IRepository;

namespace Preventyon.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApiContext _context;

        public NotificationRepository(ApiContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int employeeId)
        {
            return await _context.Notifications
                .Where(n => n.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Notification> GetNotificationByIdAsync(int notificationId)
        {
            return await _context.Notifications.FindAsync(notificationId);
        }
    }
}
