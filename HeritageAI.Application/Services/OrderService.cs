using HeritageAI.Application.Dto.Order;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly INotificationRepository _notifications;
        private readonly ICraftsmanRepository _craftsmanRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            INotificationRepository notifications,
            ICraftsmanRepository craftsmanRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _notifications = notifications;
            _craftsmanRepository = craftsmanRepository;
        }

        // الحرفي الفعلي للطلب: المنقول إليه، وإلا صاحب المنتج
        private static int? EffectiveCraftsmanId(Order o) =>
            o.AssignedCraftsmanId ?? o.Items.FirstOrDefault()?.Product.CraftsmanId;

        public async Task<OrderDto> CreateOrderAsync(int buyerId, CreateOrderDto dto)
        {
            if (dto.Items is null || dto.Items.Count == 0)
                throw new InvalidOperationException("Order must contain at least one item.");
            var city = dto.ShippingCity?.Trim();
            var address = dto.ShippingAddress?.Trim();
            var phone = dto.ContactPhone?.Trim();

            if (string.IsNullOrEmpty(city) || city.Length > 100)
                throw new InvalidOperationException("City is required (max 100 characters).");

            if (string.IsNullOrEmpty(address) || address.Length > 250)
                throw new InvalidOperationException("Address is required (max 250 characters).");

            if (string.IsNullOrEmpty(phone) ||
                !System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?[0-9\s\-]{7,20}$"))
                throw new InvalidOperationException("A valid phone number is required.");
            var order = new Order
            {
                BuyerId = buyerId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ShippingCity = city,
                ShippingAddress = address,
                ContactPhone = phone,
                Items = new List<OrderItem>()
            };

            decimal totalPrice = 0;
            int? orderCraftsmanId = null;

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);

                if (product is null)
                    throw new InvalidOperationException($"Product with id {itemDto.ProductId} not found.");

                if (product.Status != ProductStatus.Published)
                    throw new InvalidOperationException($"Product '{product.Name}' is not available for purchase.");

                if (itemDto.Quantity <= 0)
                    throw new InvalidOperationException("Quantity must be greater than zero.");

                // كل طلب لازم يكون من حرفي واحد، لأن الحرفي هو اللي بيقبل أو بيرفض
                orderCraftsmanId ??= product.CraftsmanId;
                if (product.CraftsmanId != orderCraftsmanId)
                    throw new InvalidOperationException(
                        "An order can only contain products from one craftsman. Please place a separate order for each craftsman.");

                var notes = itemDto.CustomizationNotes?.Trim();
                if (!string.IsNullOrEmpty(notes))
                {
                    if (!product.IsCustomizable)
                        throw new InvalidOperationException($"Product '{product.Name}' does not support customization.");

                    if (notes.Length > 500)
                        throw new InvalidOperationException("Customization notes must be 500 characters or fewer.");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    Price = product.Price,   // Snapshot من السعر الحالي وقت الشراء
                    CustomizationNotes = string.IsNullOrEmpty(notes) ? null : notes
                };

                order.Items.Add(orderItem);
                totalPrice += product.Price * itemDto.Quantity;
            }

            order.TotalPrice = totalPrice;
            if (orderCraftsmanId.HasValue)
            {
                _notifications.Add(new Notification
                {
                    UserId = orderCraftsmanId.Value,
                    Title = "طلب جديد بانتظار ردك",
                    Message = "وصلك طلب جديد، اقبله أو ارفضه من صفحة طلبات البيع.",
                    Link = "/orders"
                });
            }

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            var created = await _orderRepository.GetByIdAsync(order.Id);
            return MapToDto(created!);
        }

        public async Task<List<OrderDto>> GetMyOrdersAsync(int buyerId)
        {
            var orders = await _orderRepository.GetByBuyerIdAsync(buyerId);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<List<CraftsmanSaleDto>> GetMySalesAsync(int craftsmanId)
        {
            var items = await _orderRepository.GetItemsByCraftsmanIdAsync(craftsmanId);

            return items.Select(oi =>
            {
                var o = oi.Order;
                // بيانات التوصيل بتنكشف للحرفي بعد القبول فقط
                var accepted = o.Status is OrderStatus.Confirmed
                                        or OrderStatus.Shipped
                                        or OrderStatus.Delivered;

                return new CraftsmanSaleDto
                {
                    OrderId = oi.OrderId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    LineTotal = oi.Price * oi.Quantity,
                    CustomizationNotes = oi.CustomizationNotes,
                    OrderStatus = o.Status.ToString(),
                    OrderDate = o.CreatedAt,
                    BuyerName = o.Buyer.FullName,
                    ShippingCity = accepted ? o.ShippingCity : null,
                    ShippingAddress = accepted ? o.ShippingAddress : null,
                    ContactPhone = accepted ? o.ContactPhone : null
                };
            }).ToList();
        }
        // =====================================================
        // إدارة حالة الطلب: الحرفي وحده هو اللي بيحرّك الحالة
        // =====================================================

        private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
        {
            [OrderStatus.Pending] = new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
            [OrderStatus.Confirmed] = new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
            [OrderStatus.Shipped] = new[] { OrderStatus.Delivered },
            [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
            [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
        };

        private static void EnsureValidTransition(OrderStatus current, OrderStatus next)
        {
            if (current == next)
                throw new InvalidOperationException($"Order is already '{current}'.");

            if (!AllowedTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(next))
            {
                throw new InvalidOperationException(
                    $"Cannot move order from '{current}' to '{next}'. " +
                    $"Allowed next step(s): {string.Join(", ", AllowedTransitions[current])}.");
            }
        }

        // الأدمن بيشوف كل الطلبات فقط (بدون أي تعديل)
        public async Task<List<AdminOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            return orders.Select(o => new AdminOrderDto
            {
                Id = o.Id,
                Status = o.Status.ToString(),
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt,
                BuyerName = o.Buyer.FullName,
                BuyerEmail = o.Buyer.Email,
                CraftsmanId = EffectiveCraftsmanId(o) ?? 0,
                IsReassigned = o.AssignedCraftsmanId.HasValue,
                HandledByAdmin = o.HandledByAdmin,
                CraftsmanName = o.AssignedCraftsman?.User.FullName
                                ?? o.Items.FirstOrDefault()?.Product.Craftsman.User.FullName ?? "",
                RejectionReason = o.RejectionReason,
                CancelledByBuyer = o.CancelledByBuyer,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    CustomizationNotes = i.CustomizationNotes
                }).ToList()
            }).ToList();
        }
        public async Task ReassignOrderAsync(int adminId, int orderId, int newCraftsmanId, string reason)
        {
            var trimmed = reason?.Trim();
            if (string.IsNullOrEmpty(trimmed))
                throw new InvalidOperationException("Reassignment reason is required.");
            if (trimmed.Length > 300)
                throw new InvalidOperationException("Reassignment reason must be 300 characters or fewer.");

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                throw new KeyNotFoundException($"Order with id {orderId} not found.");

            if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
                throw new InvalidOperationException("Only Pending or Confirmed orders can be reassigned.");

            var oldCraftsmanId = EffectiveCraftsmanId(order)
                ?? throw new InvalidOperationException("Order has no craftsman.");

            if (oldCraftsmanId == newCraftsmanId)
                throw new InvalidOperationException("The order is already assigned to this craftsman.");

            var newCraftsman = await _craftsmanRepository.GetByUserIdAsync(newCraftsmanId);
            if (newCraftsman is null
                || newCraftsman.Status != CraftsmanStatus.Approved
                || !newCraftsman.User.IsActive)
                throw new InvalidOperationException("The selected craftsman is not available.");

            order.AssignedCraftsmanId = newCraftsmanId;
            order.Status = OrderStatus.Pending;   // الحرفي الجديد بيقبل أو بيرفض بنفسه
            order.RejectionReason = null;
            order.HandledByAdmin = false;

            await _orderRepository.AddTransferAsync(new OrderTransfer
            {
                OrderId = order.Id,
                FromCraftsmanId = oldCraftsmanId,
                ToCraftsmanId = newCraftsmanId,
                AdminId = adminId,
                Reason = trimmed
            });

            // السبب داخلي (مثلاً مرض الحرفي) فما بنعرضه بالإشعارات
            _notifications.Add(new Notification
            {
                UserId = oldCraftsmanId,
                Title = "تم سحب طلب منك",
                Message = $"نقلت الإدارة الطلب #{order.Id} إلى حرفي آخر.",
                Link = "/orders"
            });
            _notifications.Add(new Notification
            {
                UserId = newCraftsmanId,
                Title = "طلب جديد محوّل إليك",
                Message = $"حوّلت الإدارة إليك الطلب #{order.Id}. اقبله أو ارفضه من صفحة طلبات البيع.",
                Link = "/orders"
            });
            _notifications.Add(new Notification
            {
                UserId = order.BuyerId,
                Title = "تم تحويل طلبك لحرفي آخر",
                Message = $"حوّلت الإدارة طلبك #{order.Id} إلى حرفي آخر لضمان تنفيذه. بتوصلك إشعارات عند رده.",
                Link = "/my-orders"
            });

            await _orderRepository.SaveChangesAsync();
        }
        public async Task<OrderDto> CraftsmanUpdateStatusAsync(
            int craftsmanId, int orderId, OrderStatus newStatus, string? reason = null)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                throw new KeyNotFoundException($"Order with id {orderId} not found.");

            if (order.Items.Count == 0 || EffectiveCraftsmanId(order) != craftsmanId)
                throw new UnauthorizedAccessException("This order does not belong to you.");

            EnsureValidTransition(order.Status, newStatus);

            order.Status = newStatus;
            order.HandledByAdmin = false;

            if (newStatus == OrderStatus.Cancelled)
            {
                var trimmed = reason?.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    order.RejectionReason = null;
                else
                    order.RejectionReason = trimmed.Length > 300 ? trimmed[..300] : trimmed;
            }

            NotifyBuyer(order, newStatus);

            await _orderRepository.SaveChangesAsync();

            return MapToDto(order);
        }
        // الأدمن بيقبل أو بيرفض طلب Pending بالنيابة عن الحرفي، والسبب إلزامي
        public async Task AdminRespondAsync(int adminId, int orderId, bool accept, string reason)
        {
            var trimmed = reason?.Trim();
            if (string.IsNullOrEmpty(trimmed))
                throw new InvalidOperationException("Action reason is required.");
            if (trimmed.Length > 300)
                throw new InvalidOperationException("Action reason must be 300 characters or fewer.");

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                throw new KeyNotFoundException($"Order with id {orderId} not found.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only Pending orders can be accepted or rejected by admin.");

            var newStatus = accept ? OrderStatus.Confirmed : OrderStatus.Cancelled;

            order.Status = newStatus;
            order.HandledByAdmin = true;
            order.RejectionReason = accept ? null : trimmed;   // سبب الرفض بيظهر للمشتري، سبب القبول داخلي

            await _orderRepository.AddAuditLogAsync(new AuditLog
            {
                AdminId = adminId,
                Action = accept ? "OrderAccepted" : "OrderRejected",
                EntityType = "Order",
                EntityId = order.Id,
                Reason = trimmed
            });

            NotifyBuyer(order, newStatus);

            var craftsmanId = EffectiveCraftsmanId(order);
            if (craftsmanId.HasValue)
            {
                _notifications.Add(new Notification
                {
                    UserId = craftsmanId.Value,
                    Title = accept ? "الإدارة قبلت طلبًا نيابة عنك" : "الإدارة رفضت طلبًا",
                    Message = accept
                        ? $"قبلت الإدارة الطلب #{order.Id} بالنيابة عنك. تابع تجهيزه من صفحة طلبات البيع."
                        : $"رفضت الإدارة الطلب #{order.Id}.",
                    Link = "/orders"
                });
            }

            await _orderRepository.SaveChangesAsync();
        }
        // المشتري يقدر يلغي طلبه فقط وهو بانتظار الحرفي (Pending)
        public async Task<OrderDto> BuyerCancelAsync(int buyerId, int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                throw new KeyNotFoundException($"Order with id {orderId} not found.");

            if (order.BuyerId != buyerId)
                throw new UnauthorizedAccessException("This order does not belong to you.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException(
                    "This order can no longer be cancelled because the craftsman has already responded to it.");

            order.Status = OrderStatus.Cancelled;
            order.CancelledByBuyer = true;
            order.RejectionReason = null;

            var craftsmanId = EffectiveCraftsmanId(order);
            if (craftsmanId.HasValue)
            {
                _notifications.Add(new Notification
                {
                    UserId = craftsmanId.Value,
                    Title = "تم إلغاء طلب",
                    Message = $"ألغى المشتري الطلب #{order.Id} قبل ردك عليه.",
                    Link = "/orders"
                });
            }

            await _orderRepository.SaveChangesAsync();

            return MapToDto(order);
        }
        private void NotifyBuyer(Order order, OrderStatus status)
        {
            string? title = null;
            string? message = null;
            var byAdmin = order.HandledByAdmin;

            switch (status)
            {
                case OrderStatus.Confirmed:
                    title = "تم قبول طلبك";
                    message = byAdmin
                        ? $"قبلت الإدارة طلبك #{order.Id} وبدأ تجهيزه."
                        : $"الحرفي قبل طلبك #{order.Id} وبدأ بتجهيزه.";
                    break;
                case OrderStatus.Shipped:
                    title = "تم شحن طلبك";
                    message = $"طلبك #{order.Id} بالطريق إليك.";
                    break;
                case OrderStatus.Delivered:
                    title = "تم تسليم طلبك";
                    message = $"تم تسليم طلبك #{order.Id}. نتمنى أن ينال إعجابك.";
                    break;
                case OrderStatus.Cancelled:
                    title = "تعذّر تنفيذ طلبك";
                    message = (byAdmin
                                  ? $"رفضت الإدارة طلبك #{order.Id}."
                                  : $"الحرفي لم يتمكن من تنفيذ طلبك #{order.Id}.") +
                              (string.IsNullOrWhiteSpace(order.RejectionReason) ? "" : $" السبب: {order.RejectionReason}");
                    break;
            }

            if (title is null || message is null) return;

            _notifications.Add(new Notification
            {
                UserId = order.BuyerId,
                Title = title,
                Message = message,
                Link = "/my-orders"
            });
        }
        private static OrderDto MapToDto(Order o) => new()
        {
            Id = o.Id,
            Status = o.Status.ToString(),
            TotalPrice = o.TotalPrice,
            CreatedAt = o.CreatedAt,
            RejectionReason = o.RejectionReason,
            CancelledByBuyer = o.CancelledByBuyer,
            Items = o.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                Price = i.Price,
                CustomizationNotes = i.CustomizationNotes
            }).ToList()
        };
    }
}