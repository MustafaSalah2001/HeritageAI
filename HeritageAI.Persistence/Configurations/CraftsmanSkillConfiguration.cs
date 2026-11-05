using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Configurations
{
    public class CraftsmanSkillConfiguration : IEntityTypeConfiguration<CraftsmanSkill>
    {
        public void Configure(EntityTypeBuilder<CraftsmanSkill> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(s => s.Craftsman)
                .WithMany(c => c.Skills)
                .HasForeignKey(s => s.CraftsmanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
