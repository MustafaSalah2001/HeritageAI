using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        // بدون SaveChanges عن قصد: بيتحفظ مع الطلب بنفس العملية (نفس الـ DbContext)
        public void Add(Notification notification) =>
            _context.Notifications.Add(notification);

        // بتولّد إشعار لكل أدمن فعّال
        public async Task AddForAdminsAsync(string title, string message, string? link)
        {
            var adminIds = await _context.Users
                .Where(u => u.Role == UserRole.Admin && u.IsActive)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var adminId in adminIds)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = adminId,
                    Title = title,
                    Message = message,
                    Link = link
                });
            }
        }

        public async Task<List<Notification>> GetLatestAsync(int userId, int take) =>
            await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync();

        public async Task<int> CountUnreadAsync(int userId) =>
            await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task MarkReadAsync(int userId, int notificationId) =>
            await _context.Notifications
                .Where(n => n.UserId == userId && n.Id == notificationId)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

        public async Task MarkAllReadAsync(int userId) =>
            await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }
}