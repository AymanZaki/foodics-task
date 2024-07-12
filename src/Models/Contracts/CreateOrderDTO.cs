using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Contracts
{
    public class CreateOrderDTO
    {
        public List<OrderProductRequestDTO> Products { get; set; }
    }
    public class OrderProductRequestDTO
    {
        public int Product_Id { get; set; }
        public int Quantity { get; set; }
    }
}
