using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Domain.Entities
{
    public class CraftsmanSkill
    {
        public int Id { get; set; }
        public int CraftsmanId { get; set; }
        public Craftsman Craftsman { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
    }
}
