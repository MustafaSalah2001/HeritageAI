using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id) =>
            await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        public async Task<List<OrderItem>> GetItemsByCraftsmanIdAsync(int craftsmanId) =>
    await _context.OrderItems
        .Include(oi => oi.Order)
            .ThenInclude(o => o.Buyer)
            .Include(oi => oi.Product)
                .Where(oi => (oi.Order.AssignedCraftsmanId ?? oi.Product.CraftsmanId) == craftsmanId)
        .OrderByDescending(oi => oi.Order.CreatedAt)
        .ToListAsync();
        public async Task<List<Order>> GetByBuyerIdAsync(int buyerId) =>
            await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        // ← جديد: لازم للأدمن عشان يشوف كل الطلبات بالمنصة
        public async Task AddAuditLogAsync(AuditLog log) =>
    await _context.AuditLogs.AddAsync(log);
        public async Task<List<Order>> GetAllAsync() =>
          await _context.Orders
              .Include(o => o.Buyer)
              .Include(o => o.AssignedCraftsman).ThenInclude(c => c!.User)
              .Include(o => o.Items).ThenInclude(i => i.Product)
                  .ThenInclude(p => p.Craftsman).ThenInclude(c => c.User)
              .OrderByDescending(o => o.CreatedAt)
              .ToListAsync();
        public async Task AddAsync(Order order) =>
            await _context.Orders.AddAsync(order);
        public async Task AddTransferAsync(OrderTransfer transfer) =>
    await _context.OrderTransfers.AddAsync(transfer);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
