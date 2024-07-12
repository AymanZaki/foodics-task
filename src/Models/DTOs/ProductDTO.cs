using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsDeleted { get; set; }
        public List<ProductIngredientDTO> ProductIngredients { get; set; }
    }
}
