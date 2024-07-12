using Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace Entities.EFConfiguration
{
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.HasKey(f => f.IngredientId);
            builder.Property(f => f.IngredientName).HasMaxLength(256);

            builder.HasData(
            new Ingredient { IngredientId = 1, IngredientName = "Beef", Stock = 20000 },
            new Ingredient { IngredientId = 2, IngredientName = "Cheese", Stock = 20000 },
            new Ingredient { IngredientId = 3, IngredientName = "Onion", Stock = 20000 }
            );
        }
    }
}
