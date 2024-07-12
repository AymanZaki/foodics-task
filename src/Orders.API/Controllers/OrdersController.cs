using AutoMapper;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Contracts;

namespace Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderCore _orderCore;
        private readonly IMapper _mapper;
        public OrdersController(IOrderCore orderCore, IMapper mapper)
        {
            _orderCore = orderCore;
            _mapper = mapper;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateNewOrder([FromBody] CreateOrderDTO request)
        {
            var resultModel = await _orderCore.CreateOrder(request);
            if(resultModel.IsSuccess)
            {
                var orderResponse = _mapper.Map<OrderResponse>(resultModel.Data);

                var response = new ResultModel<OrderResponse>(resultModel.StatusCode, resultModel.IsSuccess, orderResponse, resultModel.Message);
                return StatusCode((int)resultModel.StatusCode, response);
            }
            return StatusCode((int)resultModel.StatusCode, resultModel);
        }
    }
}
