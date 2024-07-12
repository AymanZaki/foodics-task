using Entities;

namespace Dal.UnitOfWork
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IOrderDBContext _dbContext;

        public Repository(IOrderDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public T Add(T entity)
        {
            var entry = _dbContext.Set<T>().Add(entity);

            return entry.Entity;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
        }

        public T Delete(T entity)
        {
            var entry = _dbContext.Set<T>().Remove(entity);
            return entry.Entity;
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public T Update(T entity)
        {
            var entry = _dbContext.Set<T>().Update(entity);
            return entry.Entity;
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }
    }
}
