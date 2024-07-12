using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class IngredientStockDTO
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public double AvailabilityStock { get; set; }
        public bool EmailSent { get; set; }
        public IngredientDTO Ingredient { get; set; }
    }
}
