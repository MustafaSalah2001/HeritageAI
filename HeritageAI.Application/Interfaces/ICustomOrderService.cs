using HeritageAI.Application.Dto.CustomOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface ICustomOrderService
    {
        Task<CustomOrderDto> CreateAsync(int buyerId, CreateCustomOrderDto dto);
        Task<List<CustomOrderDto>> GetMyRequestsAsBuyerAsync(int buyerId);
        Task<List<CustomOrderDto>> GetMyRequestsAsCraftsmanAsync(int craftsmanId);
        Task<CustomOrderDto> AcceptAsync(int craftsmanId, int requestId, RespondCustomOrderDto dto);
        Task<CustomOrderDto> RejectAsync(int craftsmanId, int requestId);
    }
}
