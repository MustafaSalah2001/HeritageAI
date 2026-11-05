using HeritageAI.Application.Dto.CustomOrder;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class CustomOrderService : ICustomOrderService
    {
        private readonly ICustomOrderRepository _repository;
        private readonly ICraftsmanRepository _craftsmanRepository;
        private readonly INotificationRepository _notifications;

        // تأكد من هدول مع App.tsx
        private const string BuyerCustomOrdersLink = "/my-custom-orders";
        private const string CraftsmanCustomOrdersLink = "/craftsman/custom-orders";

        public CustomOrderService(
            ICustomOrderRepository repository,
            ICraftsmanRepository craftsmanRepository,
            INotificationRepository notifications)
        {
            _repository = repository;
            _craftsmanRepository = craftsmanRepository;
            _notifications = notifications;
        }

        public async Task<CustomOrderDto> CreateAsync(int buyerId, CreateCustomOrderDto dto)
        {
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(dto.CraftsmanId);

            if (craftsman is null)
                throw new InvalidOperationException("Craftsman not found.");

            if (craftsman.Status != CraftsmanStatus.Approved)
                throw new InvalidOperationException("This craftsman is not available for custom orders.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new InvalidOperationException("Description is required.");

            var request = new CustomOrderRequest
            {
                BuyerId = buyerId,
                CraftsmanId = dto.CraftsmanId,
                Description = dto.Description,
                DesiredColors = dto.DesiredColors,
                Size = dto.Size,
                Status = CustomOrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(request);

            _notifications.Add(new Notification
            {
                UserId = dto.CraftsmanId,
                Title = "طلب تصميم مخصص جديد",
                Message = "وصلك طلب مخصص جديد. شوف التفاصيل وردّ عليه بسعر ومدة التنفيذ.",
                Link = CraftsmanCustomOrdersLink
            });

            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(request.Id);
            return MapToDto(created!);
        }

        public async Task<List<CustomOrderDto>> GetMyRequestsAsBuyerAsync(int buyerId)
        {
            var requests = await _repository.GetByBuyerIdAsync(buyerId);
            return requests.Select(MapToDto).ToList();
        }

        public async Task<List<CustomOrderDto>> GetMyRequestsAsCraftsmanAsync(int craftsmanId)
        {
            var requests = await _repository.GetByCraftsmanIdAsync(craftsmanId);
            return requests.Select(MapToDto).ToList();
        }

        public async Task<CustomOrderDto> AcceptAsync(int craftsmanId, int requestId, RespondCustomOrderDto dto)
        {
            var request = await _repository.GetByIdAsync(requestId);

            if (request is null)
                throw new KeyNotFoundException("Request not found.");

            if (request.CraftsmanId != craftsmanId)
                throw new UnauthorizedAccessException("You can only respond to your own requests.");

            if (request.Status != CustomOrderStatus.Pending)
                throw new InvalidOperationException("This request has already been answered.");

            if (dto.ProposedPrice <= 0)
                throw new InvalidOperationException("Proposed price must be greater than zero.");

            request.Status = CustomOrderStatus.Accepted;
            request.ProposedPrice = dto.ProposedPrice;
            request.EstimatedDurationDays = dto.EstimatedDurationDays;

            _notifications.Add(new Notification
            {
                UserId = request.BuyerId,
                Title = "وصلك عرض على طلبك المخصص",
                Message = $"قدّم {request.Craftsman.User.FullName} عرضًا بسعر {dto.ProposedPrice}₪ لطلبك المخصص.",
                Link = BuyerCustomOrdersLink
            });

            await _repository.SaveChangesAsync();

            return MapToDto(request);
        }

        public async Task<CustomOrderDto> RejectAsync(int craftsmanId, int requestId)
        {
            var request = await _repository.GetByIdAsync(requestId);

            if (request is null)
                throw new KeyNotFoundException("Request not found.");

            if (request.CraftsmanId != craftsmanId)
                throw new UnauthorizedAccessException("You can only respond to your own requests.");

            if (request.Status != CustomOrderStatus.Pending)
                throw new InvalidOperationException("This request has already been answered.");

            request.Status = CustomOrderStatus.Rejected;

            _notifications.Add(new Notification
            {
                UserId = request.BuyerId,
                Title = "تعذّر تنفيذ طلبك المخصص",
                Message = $"اعتذر {request.Craftsman.User.FullName} عن تنفيذ طلبك المخصص. تقدر تجرّب حرفي ثاني.",
                Link = BuyerCustomOrdersLink
            });

            await _repository.SaveChangesAsync();

            return MapToDto(request);
        }

        private static CustomOrderDto MapToDto(CustomOrderRequest c) => new()
        {
            Id = c.Id,
            BuyerId = c.BuyerId,
            BuyerName = c.Buyer.FullName,
            CraftsmanId = c.CraftsmanId,
            CraftsmanName = c.Craftsman.User.FullName,
            Description = c.Description,
            DesiredColors = c.DesiredColors,
            Size = c.Size,
            Status = c.Status.ToString(),
            ProposedPrice = c.ProposedPrice,
            EstimatedDurationDays = c.EstimatedDurationDays,
            CreatedAt = c.CreatedAt
        };
    }
}