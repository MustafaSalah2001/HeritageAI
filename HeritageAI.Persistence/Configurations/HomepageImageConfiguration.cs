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
    public class HomepageImageConfiguration : IEntityTypeConfiguration<HomepageImage>
    {
        public void Configure(EntityTypeBuilder<HomepageImage> builder)
        {
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Url).IsRequired();
        }
    }
}
