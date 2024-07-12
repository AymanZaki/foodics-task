

using Entities;
using Entities.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dal.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IOrderDBContext _dbContext;
        private IDbContextTransaction _transaction;

        public IRepository<Order> OrderRepository { get; }
        public IRepository<Ingredient> IngredientRepository { get; }
        public IRepository<IngredientStock> IngredientStockRepository { get; }
        public IRepository<Product> ProductRepository { get; }
        public IRepository<ProductIngredient> ProductIngredientRepository { get; }
        public IRepository<OrderProduct> OrderProductRepository { get; }

        public UnitOfWork(IOrderDBContext dbContext,
            IRepository<Order> orderRepository,
            IRepository<Ingredient> ingredientRepository,
            IRepository<Product> productRepository,
            IRepository<ProductIngredient> productIngredientRepository,
            IRepository<OrderProduct> orderProductRepository,
            IRepository<IngredientStock> ingredientStockRepository
            )
        {
            _dbContext = dbContext;
            OrderRepository = orderRepository;
            IngredientRepository = ingredientRepository;
            ProductRepository = productRepository;
            ProductIngredientRepository = productIngredientRepository;
            OrderProductRepository = orderProductRepository;
            IngredientStockRepository = ingredientStockRepository;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await CommitAsync();
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                var processId = await _dbContext.SaveChangesAsync();
                return processId > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        public async void Dispose()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
