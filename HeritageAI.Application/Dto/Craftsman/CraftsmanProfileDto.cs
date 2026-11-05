using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Dto.Craftsman
{
    public class CraftsmanProfileDto
    {
        public List<string> Skills { get; set; } = new();
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string? StoryText { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
