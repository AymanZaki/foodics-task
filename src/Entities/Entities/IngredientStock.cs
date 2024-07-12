using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class IngredientStock
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public double AvailabilityStock { get; set; }
        public bool EmailSent { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}
