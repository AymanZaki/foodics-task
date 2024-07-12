using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<ProductIngredient> ProductIngredients { get; set; }
    }
}
