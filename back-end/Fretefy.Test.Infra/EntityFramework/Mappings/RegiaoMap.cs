using Fretefy.Test.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fretefy.Test.Infra.EntityFramework.Mappings
{
    public class RegiaoMap : IEntityTypeConfiguration<Regiao>
    {
        public void Configure(EntityTypeBuilder<Regiao> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Nome).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Status).IsRequired().HasDefaultValue(true);

            builder.HasMany(r => r.RegiaoCidades)
                .WithOne(rc => rc.Regiao)
                .HasForeignKey(rc => rc.RegiaoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
