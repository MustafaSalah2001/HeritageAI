using HeritageAI.Application.Dto.Notifications;
using HeritageAI.Application.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<NotificationsResponseDto> GetMineAsync(int userId)
        {
            var items = await _repository.GetLatestAsync(userId, 30);
            var unread = await _repository.CountUnreadAsync(userId);

            return new NotificationsResponseDto
            {
                UnreadCount = unread,
                Items = items.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                }).ToList()
            };
        }

        public Task MarkReadAsync(int userId, int notificationId) =>
            _repository.MarkReadAsync(userId, notificationId);

        public Task MarkAllReadAsync(int userId) =>
            _repository.MarkAllReadAsync(userId);
    }
}