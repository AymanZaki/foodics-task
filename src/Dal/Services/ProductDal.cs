using Dal.Interfaces;
using Entities;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Services
{
    public class ProductDal : IProductDal
    {
        private readonly IOrderDBContext _dbContext;
        public ProductDal(IOrderDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetProductById(int id, IEnumerable<string>? includes = null)
        {
            var query = _dbContext.Product.AsNoTracking();
            if(includes != null)
            {
                foreach(string include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(x => x.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids, IEnumerable<string> includes = null)
        {
            var query = _dbContext.Product.AsQueryable();
            if (includes != null)
            {
                foreach (string include in includes)
                {
                    query = query.Include(include);
                }
            }
            query = query.Where(x => ids.Contains(x.ProductId) && !x.IsDeleted);
            return await query.ToListAsync();
        }
    }
}
