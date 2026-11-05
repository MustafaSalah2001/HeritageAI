using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.Order
{
    public class CreateOrderDto
    {
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;

        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
