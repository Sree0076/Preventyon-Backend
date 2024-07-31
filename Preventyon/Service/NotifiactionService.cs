using AutoMapper;
using Preventyon.Models;
using Preventyon.Models.DTO.Notification;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;

namespace Preventyon.Service
{
    public class NotifiactionService
    {
        public class NotificationService : INotificationService
        {
            private readonly INotificationRepository _notificationRepository;
            private readonly IEmployeeRepository _employeeRepository;
            private readonly IMapper _mapper;

            public NotificationService(INotificationRepository notificationRepository, IEmployeeRepository employeeRepository, IMapper mapper)
            {
                _notificationRepository = notificationRepository;
                _employeeRepository = employeeRepository;
                _mapper = mapper;
            }

            public async Task SendNotificationAsync(CreateNotificationDTO notificationDto, IEnumerable<int> employeeIds)
            {
                foreach (var employeeId in employeeIds)
                {
                    var notification = _mapper.Map<Notification>(notificationDto);
                    notification.EmployeeId = employeeId;
                    await _notificationRepository.AddNotificationAsync(notification);
                }
            }

            public async Task<IEnumerable<GetNotificationDTO>> GetUserNotificationsAsync(int employeeId)
            {
                var notifications = await _notificationRepository.GetNotificationsForUserAsync(employeeId);
                return _mapper.Map<IEnumerable<GetNotificationDTO>>(notifications);
            }

            public async Task MarkNotificationAsReadAsync(int notificationId)
            {
                await _notificationRepository.MarkAsReadAsync(notificationId);
            }
            public async Task<Notification> GetNotificationByIdAsync(int notificationId) // Implementation
            {
                return await _notificationRepository.GetNotificationByIdAsync(notificationId);
            }




            public async Task NotifyRoleChangeAsync(int userId, string newRole)
            {
                var notificationDto = new CreateNotificationDTO
                {
                    IncidentId = 0,
                    Type = "RoleChange"
                };

                await SendNotificationAsync(notificationDto, new List<int> { userId });
            }

            public async Task NotifyPermissionChangeAsync(int userId, List<string> permissions)
            {
                var permissionsText = string.Join(", ", permissions);
                var notificationDto = new CreateNotificationDTO
                {
                    IncidentId = 0,
                    Type = "PermissionChange"
                };

                await SendNotificationAsync(notificationDto, new List<int> { userId });
            }
        }
    }
}
