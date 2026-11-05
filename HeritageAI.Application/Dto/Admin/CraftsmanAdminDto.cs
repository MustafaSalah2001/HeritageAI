using System;
using System.Collections.Generic;

namespace HeritageAI.Application.Dto.Admin
{
    public class CraftsmanAdminDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public int ProductsCount { get; set; }
        public string? StoryText { get; set; }
        public string? ProfileImageUrl { get; set; }
        public List<string> Skills { get; set; } = new();
        public List<CraftsmanWorkDto> Works { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CraftsmanWorkDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
    }
}