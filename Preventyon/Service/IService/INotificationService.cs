using Preventyon.Models;
using Preventyon.Models.DTO.Notification;

namespace Preventyon.Service.IService
{
    public interface INotificationService
    {
        Task SendNotificationAsync(CreateNotificationDTO notificationDto, IEnumerable<int> employeeIds);
        Task<IEnumerable<GetNotificationDTO>> GetUserNotificationsAsync(int employeeId);
        Task MarkNotificationAsReadAsync(int notificationId);
     
     
        Task<Notification> GetNotificationByIdAsync(int notificationId);
    }
}
