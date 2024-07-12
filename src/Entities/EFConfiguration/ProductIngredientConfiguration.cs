using Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.EFConfiguration
{
    internal class ProductIngredientConfiguration : IEntityTypeConfiguration<ProductIngredient>
    {
        public void Configure(EntityTypeBuilder<ProductIngredient> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasOne(f => f.Product)
                .WithMany(x => x.ProductIngredients)
                .HasForeignKey(b => b.ProductId);

            builder.HasOne(f => f.Ingredient)
                .WithMany()
                .HasForeignKey(b => b.IngredientId);

            builder.HasData(
                new ProductIngredient { Id = 1, ProductId = 1, IngredientId = 1, Quantity = 150 },
                new ProductIngredient { Id = 2, ProductId = 1, IngredientId = 2, Quantity = 30 },
                new ProductIngredient { Id = 3, ProductId = 1, IngredientId = 3, Quantity = 20 }
                );
        }

    }
}
