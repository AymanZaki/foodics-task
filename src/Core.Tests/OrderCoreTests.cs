using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Core.Interfaces;
using Core.Services;
using Dal.Interfaces;
using Dal.UnitOfWork;
using Entities.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Models;
using Models.Contracts;
using Models.DTOs;
using Xunit;

namespace Core.Tests.Services
{
    public class OrderCoreTests
    {
        private readonly Mock<IOrderDal> _orderDal;
        private readonly Mock<IProductDal> _productDal;
        private readonly Mock<IIngredientDal> _ingredientDal;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ILogger<OrderCore>> _logger;
        private readonly Mock<IEmailService> _emailService;
        private readonly OrderCore _orderCore;

        public OrderCoreTests()
        {
            _orderDal = new Mock<IOrderDal>();
            _productDal = new Mock<IProductDal>();
            _ingredientDal = new Mock<IIngredientDal>();
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _logger = new Mock<ILogger<OrderCore>>();
            _emailService = new Mock<IEmailService>();

            _orderCore = new OrderCore(
                _orderDal.Object,
                _productDal.Object,
                _ingredientDal.Object,
                _mapper.Object,
                _logger.Object,
                _unitOfWork.Object,
                _emailService.Object
            );
        }

        [Fact]
        public async Task CreateOrder_InvalidProductIds_ReturnsBadRequest()
        {
            // Arrange
            var createOrderDTO = new CreateOrderDTO
            {
                Products = new List<OrderProductRequestDTO>
                {
                    new OrderProductRequestDTO { Product_Id = 1, Quantity = 2 },
                    new OrderProductRequestDTO { Product_Id = 2, Quantity = 3 }
                }
            };

            _productDal.Setup(x => x.GetProductsByIds(It.IsAny<List<int>>(), It.IsAny<List<string>>()))
                           .ReturnsAsync((IEnumerable<Product>?)null);

            // Act
            var result = await _orderCore.CreateOrder(createOrderDTO);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CreateOrder_InsufficientStock_ReturnsBadRequest()
        {
            // Arrange
            var createOrderDTO = new CreateOrderDTO
            {
                Products = new List<OrderProductRequestDTO>
                {
                    new OrderProductRequestDTO { Product_Id = 1, Quantity = 2 }
                }
            };

            var products = new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductIngredients = new List<ProductIngredient>
                    {
                        new ProductIngredient { IngredientId = 1, Quantity = 3 }
                    }
                }
            };

            var ingredientStocks = new List<IngredientStock>
            {
                new IngredientStock { IngredientId = 1, AvailabilityStock = 2 }
            };

            _productDal.Setup(x => x.GetProductsByIds(It.IsAny<List<int>>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(products);

            _ingredientDal.Setup(x => x.GetIngredientStocksByIds(It.IsAny<List<int>>()))
                              .ReturnsAsync(ingredientStocks);

            _mapper.Setup(m => m.Map<List<IngredientStockDTO>>(It.IsAny<List<IngredientStock>>()))
                       .Returns(new List<IngredientStockDTO>
                       {
                           new IngredientStockDTO { IngredientId = 1, AvailabilityStock = 2 }
                       });

            _mapper.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>()))
                       .Returns(new List<ProductDTO>
                       {
                           new ProductDTO
                           {
                               ProductId = 1,
                               ProductIngredients = new List<ProductIngredientDTO>
                               {
                                   new ProductIngredientDTO { IngredientId = 1, Quantity = 3 }
                               }
                           }
                       });

            // Act
            var result = await _orderCore.CreateOrder(createOrderDTO);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CreateOrder_SendsLowStockEmail_WhenIngredientStockIsLow()
        {
            // Arrange
            var createOrderDTO = new CreateOrderDTO
            {
                Products = new List<OrderProductRequestDTO>
                {
                    new OrderProductRequestDTO { Product_Id = 1, Quantity = 2 }
                }
            };

            var products = new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductIngredients = new List<ProductIngredient>
                    {
                        new ProductIngredient { IngredientId = 1, Quantity = 1 }
                    }
                }
            };

            var ingredientStocks = new List<IngredientStock>
            {
                new IngredientStock 
                { 
                    IngredientId = 1,
                    AvailabilityStock = 10,
                    EmailSent = false,
                    Ingredient = new Ingredient { Stock = 10 }
                }
            };

            _productDal.Setup(x => x.GetProductsByIds(It.IsAny<List<int>>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(products);

            _ingredientDal.Setup(x => x.GetIngredientStocksByIds(It.IsAny<List<int>>()))
                              .ReturnsAsync(ingredientStocks);

            _mapper.Setup(m => m.Map<List<IngredientStockDTO>>(It.IsAny<List<IngredientStock>>()))
                       .Returns(new List<IngredientStockDTO>
                       {
                           new IngredientStockDTO { IngredientId = 1, AvailabilityStock = 10 }
                       });

            _mapper.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>()))
                       .Returns(new List<ProductDTO>
                       {
                           new ProductDTO
                           {
                               ProductId = 1,
                               ProductIngredients = new List<ProductIngredientDTO>
                               {
                                   new ProductIngredientDTO { IngredientId = 1, Quantity = 1 }
                               }
                           }
                       });

            _mapper.Setup(m => m.Map<Order>(It.IsAny<Dictionary<int, int>>()))
                       .Returns(new Order());

            _unitOfWork.Setup(x => x.IngredientStockRepository.UpdateRange(It.IsAny<List<IngredientStock>>()));

            _unitOfWork.Setup(x => x.OrderRepository.Add(It.IsAny<Order>()));

            _unitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(true);

            // Act
            var result = await _orderCore.CreateOrder(createOrderDTO);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.True(result.IsSuccess);
            Assert.Equal(ingredientStocks.First()?.AvailabilityStock, 8);
            _emailService.Verify(x => x.SendLowStockEmail(It.IsAny<List<IngredientStockDTO>>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrder_IngredientIsBelow50Percentage_ReturnsCreated()
        {
            // Arrange
            var createOrderDTO = new CreateOrderDTO
            {
                Products = new List<OrderProductRequestDTO>
                {
                    new OrderProductRequestDTO { Product_Id = 1, Quantity = 2 }
                }
            };

            var products = new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductIngredients = new List<ProductIngredient>
                    {
                        new ProductIngredient { IngredientId = 1, Quantity = 4 }
                    }
                }
            };

            var ingredientStocks = new List<IngredientStock>
            {
                new IngredientStock
                {
                    IngredientId = 1,
                    AvailabilityStock = 10,
                    EmailSent = false,
                    Ingredient = new Ingredient { Stock = 10 }
                }
            };

            _productDal.Setup(x => x.GetProductsByIds(It.IsAny<List<int>>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(products);

            _ingredientDal.Setup(x => x.GetIngredientStocksByIds(It.IsAny<List<int>>()))
                              .ReturnsAsync(ingredientStocks);

            _mapper.Setup(m => m.Map<List<IngredientStockDTO>>(It.IsAny<List<IngredientStock>>()))
                       .Returns(new List<IngredientStockDTO>
                       {
                           new IngredientStockDTO { IngredientId = 1, AvailabilityStock = 10 }
                       });

            _mapper.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>()))
                       .Returns(new List<ProductDTO>
                       {
                           new ProductDTO
                           {
                               ProductId = 1,
                               ProductIngredients = new List<ProductIngredientDTO>
                               {
                                   new ProductIngredientDTO { IngredientId = 1, Quantity = 4, ProductId = 1 }
                               }
                           }
                       });

            _mapper.Setup(m => m.Map<Order>(It.IsAny<Dictionary<int, int>>()))
                       .Returns(new Order());
            _emailService.Setup(e => e.SendLowStockEmail(It.IsAny<List<IngredientStockDTO>>()))
                       .ReturnsAsync(true);

            _unitOfWork.Setup(x => x.IngredientStockRepository.UpdateRange(It.IsAny<List<IngredientStock>>()));

            _unitOfWork.Setup(x => x.OrderRepository.Add(It.IsAny<Order>()));

            _unitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(true);

            // Act
            var result = await _orderCore.CreateOrder(createOrderDTO);

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.True(result.IsSuccess);
            Assert.True(ingredientStocks.First().EmailSent);
            Assert.Equal(ingredientStocks.First()?.AvailabilityStock, 2);
            _emailService.Verify(x => x.SendLowStockEmail(It.IsAny<List<IngredientStockDTO>>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_FailsToCommit_ReturnsInternalServerError()
        {
            // Arrange
            var createOrderDTO = new CreateOrderDTO
            {
                Products = new List<OrderProductRequestDTO>
                {
                    new OrderProductRequestDTO { Product_Id = 1, Quantity = 2 }
                }
            };

            var products = new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductIngredients = new List<ProductIngredient>
                    {
                        new ProductIngredient { IngredientId = 1, Quantity = 1 }
                    }
                }
            };

            var ingredientStocks = new List<IngredientStock>
            {
                new IngredientStock
                {
                    IngredientId = 1,
                    AvailabilityStock = 10,
                    EmailSent = false,
                    Ingredient = new Ingredient { Stock = 10 }
                }
            };

            _productDal.Setup(x => x.GetProductsByIds(It.IsAny<List<int>>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(products);

            _ingredientDal.Setup(x => x.GetIngredientStocksByIds(It.IsAny<List<int>>()))
                              .ReturnsAsync(ingredientStocks);

            _mapper.Setup(m => m.Map<List<IngredientStockDTO>>(It.IsAny<List<IngredientStock>>()))
                       .Returns(new List<IngredientStockDTO>
                       {
                           new IngredientStockDTO { IngredientId = 1, AvailabilityStock = 10 }
                       });

            _mapper.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>()))
                       .Returns(new List<ProductDTO>
                       {
                           new ProductDTO
                           {
                               ProductId = 1,
                               ProductIngredients = new List<ProductIngredientDTO>
                               {
                                   new ProductIngredientDTO { IngredientId = 1, Quantity = 1, ProductId = 1 }
                               }
                           }
                       });

            _mapper.Setup(m => m.Map<Order>(It.IsAny<Dictionary<int, int>>()))
                       .Returns(new Order());
            _emailService.Setup(e => e.SendLowStockEmail(It.IsAny<List<IngredientStockDTO>>()))
                       .ReturnsAsync(true);

            _unitOfWork.Setup(x => x.IngredientStockRepository.UpdateRange(It.IsAny<List<IngredientStock>>()));

            _unitOfWork.Setup(x => x.OrderRepository.Add(It.IsAny<Order>()));

            _unitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(false);

            // Act
            var result = await _orderCore.CreateOrder(createOrderDTO);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.IsSuccess);
            _emailService.Verify(x => x.SendLowStockEmail(It.IsAny<List<IngredientStockDTO>>()), Times.Never);
        }
    }
}
