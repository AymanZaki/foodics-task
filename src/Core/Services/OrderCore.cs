using AutoMapper;
using Core.Interfaces;
using Dal.Interfaces;
using Dal.UnitOfWork;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Contracts;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Core.Services
{
    public class OrderCore : IOrderCore
    {
        private readonly IOrderDal _orderDal;
        private readonly IProductDal _productDal;
        private readonly IIngredientDal _ingredientDal;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderCore> _logger;
        private readonly IEmailService _emailService;
        public OrderCore(IOrderDal orderDal, IProductDal productDal, IIngredientDal ingredientDal, IMapper mapper,
            ILogger<OrderCore> logger, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _orderDal = orderDal;
            _productDal = productDal;
            _ingredientDal = ingredientDal;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }
        public async Task<ResultModel<OrderDTO>> CreateOrder(CreateOrderDTO createOrderDTO)
        {
            var uniqueProductIds = createOrderDTO.Products
                                .Select(x => x.Product_Id)
                                .Distinct()
                                .ToList();
            var productIncludes = new List<string> { "ProductIngredients" };
            var products = await _productDal.GetProductsByIds(uniqueProductIds, productIncludes);
            if(products is null || uniqueProductIds.Count != products.Count())
            {
                return new ResultModel<OrderDTO>(HttpStatusCode.BadRequest, false, null, "Invalid Product Ids");
            }
            var ingredientIds = products.SelectMany(x => x.ProductIngredients.Select(pi => pi.IngredientId).ToList()).ToList();
            var ingredientStocks = (await _ingredientDal.GetIngredientStocksByIds(ingredientIds)).ToList();
            var ingredientStocksDTO = _mapper.Map<List<IngredientStockDTO>>(ingredientStocks);
            var productsDTO = _mapper.Map<List<ProductDTO>>(products);
            Dictionary<int, int> orderProductQuantities = createOrderDTO.Products
                .GroupBy(x => x.Product_Id)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Quantity));
            if (!ValidateIngredientsAvailability(orderProductQuantities, productsDTO, ingredientStocksDTO))
            {
                return new ResultModel<OrderDTO>(HttpStatusCode.BadRequest, false, null, "Insufficient stock");
            }
            UpdateIngredientStock(orderProductQuantities, productsDTO, ingredientStocks);
            var lowStockIngredient = ingredientStocks
                .Where(x => x.AvailabilityStock <= 0.5 * x.Ingredient.Stock && !x.EmailSent)
                .ToList();

            if(lowStockIngredient.Any())
            {
                var lowStockIngredientsDTO = _mapper.Map<List<IngredientStockDTO>>(lowStockIngredient);
                var isSent = await _emailService.SendLowStockEmail(lowStockIngredientsDTO);
                if (isSent)
                    lowStockIngredient = lowStockIngredient.Select(x => { x.EmailSent = true; return x; }).ToList();
            }
            var order = _mapper.Map<Order>(orderProductQuantities);
            _unitOfWork.IngredientStockRepository.UpdateRange(ingredientStocks);

            _unitOfWork.OrderRepository.Add(order);

            bool committed = await _unitOfWork.CommitAsync();
            if (!committed)
                return new ResultModel<OrderDTO>(HttpStatusCode.InternalServerError, false, null, "Failed to create the order");

            var orderDTO = _mapper.Map<OrderDTO>(order);
            return new ResultModel<OrderDTO>(HttpStatusCode.Created, true, orderDTO);
        }

        bool ValidateIngredientsAvailability(Dictionary<int, int> orderProductQuantities, List<ProductDTO> productsDTO, List<IngredientStockDTO> ingredientStocksDTO)
        {
            foreach (var orderProduct in orderProductQuantities)
            {
                var productIngredients = productsDTO
                                    .FirstOrDefault(x => x.ProductId == orderProduct.Key)
                                    ?.ProductIngredients;
                foreach (var productIngredient in productIngredients)
                {
                    var ingredient = ingredientStocksDTO.FirstOrDefault(x => x.IngredientId == productIngredient.IngredientId);
                    ingredient.AvailabilityStock -= productIngredient.Quantity * orderProduct.Value;
                    if(ingredient.AvailabilityStock < 0)
                    {
                        _logger.Log(LogLevel.Error, $"Insufficient stock for ProductId: {orderProduct.Key}");
                        return false;
                    }
                }
            }
            return true;
        }

        void UpdateIngredientStock(Dictionary<int, int> orderProductQuantities, List<ProductDTO> productsDTO, List<IngredientStock> ingredientStocks)
        {
            foreach (var orderProduct in orderProductQuantities)
            {
                var productIngredients = productsDTO
                                    .FirstOrDefault(x => x.ProductId == orderProduct.Key)
                                    ?.ProductIngredients;
                foreach (var productIngredient in productIngredients)
                {
                    var ingredientStock = ingredientStocks.FirstOrDefault(x => x.IngredientId == productIngredient.IngredientId);
                    ingredientStock.AvailabilityStock -= productIngredient.Quantity * orderProduct.Value;
                }
            }
        }
    }
}
