using Fretefy.Test.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fretefy.Test.Infra.EntityFramework.Mappings
{
    public class RegiaoCidadeMap : IEntityTypeConfiguration<RegiaoCidade>
    {
        public void Configure(EntityTypeBuilder<RegiaoCidade> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.RegiaoId).IsRequired();
            builder.Property(p => p.CidadeId).IsRequired();

            builder.HasOne(rc => rc.Regiao)
                .WithMany() 
                .HasForeignKey(rc => rc.RegiaoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rc => rc.Cidade)
                .WithMany()
                .HasForeignKey(rc => rc.CidadeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
