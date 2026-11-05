using HeritageAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetByBuyerIdAsync(int buyerId);
        Task AddTransferAsync(OrderTransfer transfer);
        Task AddAuditLogAsync(AuditLog log);
        Task<List<OrderItem>> GetItemsByCraftsmanIdAsync(int craftsmanId);   // ← جديد
        Task<List<Order>> GetAllAsync();   // ← جديد: لعرض كل الطلبات من لوحة الأدمن
        Task AddAsync(Order order);
        Task SaveChangesAsync();
    }
}
