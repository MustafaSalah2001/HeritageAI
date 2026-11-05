using HeritageAI.Application.Dto.Order;
using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int buyerId, CreateOrderDto dto);
        Task AdminRespondAsync(int adminId, int orderId, bool accept, string reason);
        Task<List<OrderDto>> GetMyOrdersAsync(int buyerId);
        Task<List<CraftsmanSaleDto>> GetMySalesAsync(int craftsmanId);   // ← جديد

        Task ReassignOrderAsync(int adminId, int orderId, int newCraftsmanId, string reason);
        // ===== إدارة حالة الطلب (جديد) =====
        Task<OrderDto> BuyerCancelAsync(int buyerId, int orderId);
        Task<List<AdminOrderDto>> GetAllOrdersAsync();
        Task<OrderDto> CraftsmanUpdateStatusAsync(int craftsmanId, int orderId, OrderStatus newStatus, string? reason = null);
    }
}