using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.Order
{
    // الحالة بتوصل كنص ("Confirmed", "Shipped", ...) بدل رقم الـ enum،
    // عشان تطابق نفس القيم اللي الفرونت شايفها بالفعل بالـ OrderDto.Status.
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}