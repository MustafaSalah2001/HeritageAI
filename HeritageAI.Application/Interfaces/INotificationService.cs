using HeritageAI.Application.Dto.Notifications;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationsResponseDto> GetMineAsync(int userId);
        Task MarkReadAsync(int userId, int notificationId);
        Task MarkAllReadAsync(int userId);
    }
}