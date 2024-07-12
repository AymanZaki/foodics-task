using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class IngredientDTO
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public double Stock { get; set; }
    }
}
