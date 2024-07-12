using Entities.Entities;
using Models;
using Models.Contracts;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOrderCore
    {
        Task<ResultModel<OrderDTO>> CreateOrder(CreateOrderDTO createOrderDTO);
    }
}
