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
    public class SiteContentConfiguration : IEntityTypeConfiguration<SiteContent>
    {
        public void Configure(EntityTypeBuilder<SiteContent> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.AboutUsText)
                .HasMaxLength(3000);

            builder.Property(s => s.ContactEmail)
                .HasMaxLength(256);

            builder.Property(s => s.ContactPhone)
                .HasMaxLength(20);
        }
    }
}
