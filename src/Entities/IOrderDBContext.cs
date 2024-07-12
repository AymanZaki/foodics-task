using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IOrderDBContext : IAsyncDisposable
    {

        DbSet<Ingredient> Ingredient { get; set; }
        DbSet<IngredientStock> IngredientStock { get; set; }
        DbSet<Order> Order { get; set; }
        DbSet<OrderProduct> OrderProduct { get; set; }
        DbSet<Product> Product { get; set; }
        DbSet<ProductIngredient> ProductIngredient { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        DatabaseFacade Database { get; }
    }
}
