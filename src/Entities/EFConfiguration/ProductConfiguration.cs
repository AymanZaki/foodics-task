using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.EFConfiguration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(f => f.ProductId);
            builder.Property(f => f.ProductName).HasMaxLength(256);


            builder.HasMany(f => f.ProductIngredients)
                .WithOne(x => x.Product)
                .HasForeignKey(b => b.ProductId);

            builder.HasData(
                new Product { ProductId = 1, ProductName = "Burger", IsDeleted = false }
                );
        }

    }
}
