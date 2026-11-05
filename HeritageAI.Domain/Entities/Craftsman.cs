using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Domain.Entities
{
    public class Craftsman
    {
        public int UserId { get; set; } // نفس الوقت PK و FK
        public User User { get; set; } = null!;

        public string Bio { get; set; } = string.Empty;
        public string? StoryText { get; set; }
        public string? ProfileImageUrl { get; set; }
        public CraftsmanStatus Status { get; set; } = CraftsmanStatus.Pending;
        public ICollection<CraftsmanSkill> Skills { get; set; } = new List<CraftsmanSkill>();
    }
}
