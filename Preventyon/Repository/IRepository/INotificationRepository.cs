using Preventyon.Models;

namespace Preventyon.Repository.IRepository
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int employeeId);
        Task MarkAsReadAsync(int notificationId);
        Task<Notification> GetNotificationByIdAsync(int notificationId);
    }
}
