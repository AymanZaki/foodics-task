using Entities.Entities;

namespace Dal.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Order> OrderRepository { get; }
        IRepository<IngredientStock> IngredientStockRepository { get; }
        IRepository<Ingredient> IngredientRepository { get; }
        IRepository<Product> ProductRepository { get; }
        IRepository<ProductIngredient> ProductIngredientRepository { get; }
        IRepository<OrderProduct> OrderProductRepository { get; }
        Task<bool> CommitAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackAsync();
    }
}
