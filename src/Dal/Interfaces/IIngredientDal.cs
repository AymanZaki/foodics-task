﻿using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Interfaces
{
    public interface IIngredientDal
    {
        Task<IEnumerable<IngredientStock>> GetIngredientStocksByIds(List<int> ingredientIds);
    }
}
