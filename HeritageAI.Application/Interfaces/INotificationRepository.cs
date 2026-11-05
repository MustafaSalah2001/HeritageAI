using HeritageAI.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface INotificationRepository
    {
        void Add(Notification notification);
        Task<List<Notification>> GetLatestAsync(int userId, int take);
        Task<int> CountUnreadAsync(int userId);
        Task MarkReadAsync(int userId, int notificationId);
        Task AddForAdminsAsync(string title, string message, string? link);
        Task MarkAllReadAsync(int userId);
    }
}