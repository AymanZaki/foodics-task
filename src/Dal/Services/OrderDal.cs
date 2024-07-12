using Dal.Interfaces;
using Entities;
using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Services
{
    public class OrderDal : IOrderDal
    {
        private IOrderDBContext _dbContext;
        public OrderDal(IOrderDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task AddOrder(Order entity)
        {
            _dbContext.Order.Add(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
