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
    public class IngredientDal : IIngredientDal
    {
        private IOrderDBContext _dbContext;
        public IngredientDal(IOrderDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<IngredientStock>> GetIngredientStocksByIds(List<int> ingredientIds)
        {
            return await _dbContext.IngredientStock
                .Include(x => x.Ingredient)
                .Where(x => ingredientIds.Contains(x.IngredientId)).ToListAsync();
        }
    }
}
