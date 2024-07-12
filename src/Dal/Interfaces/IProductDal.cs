using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Interfaces
{
    public interface IProductDal
    {
        Task<Product?> GetProductById(int id, IEnumerable<string> includes = null);
        Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids, IEnumerable<string> includes = null);
    }
}
