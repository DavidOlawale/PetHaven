using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly OrdersController _controller;
        private readonly int _testUserId = 5;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new OrdersController(_mockOrderService.Object, _mockUserService.Object);

            // simulate authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Setup ControllerContext with HttpContext
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetOrders_ReturnsOkWithAllOrders()
        {
            // Arrange
            var expectedOrders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    UserId = 1,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = 100.00m
                },
                new Order
                {
                    Id = 2,
                    UserId = 2,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Processing,
                    TotalAmount = 150.00m
                }
            };

            _mockOrderService.Setup(s => s.GetAllOrdersAsync())
                .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(expectedOrders, returnedOrders);
        }

        [Fact]
        public async Task GetOrder_ReturnsOkWithOrder_WhenOrderExistsAndBelongsToUser()
        {
            // Arrange
            int orderId = 1;
            var expectedOrder = new Order
            {
                Id = orderId,
                UserId = _testUserId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 100.00m
            };

            var user = new User { Id = _testUserId, Email = "test@example.com" };

            _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId))
                .ReturnsAsync(expectedOrder);
            _mockUserService.Setup(s => s.GetUserByIdAsync(_testUserId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrder = Assert.IsType<Order>(okResult.Value);
            Assert.Equal(expectedOrder, returnedOrder);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 999;
            var user = new User { Id = _testUserId, Email = "test@example.com" };

            _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);
            _mockUserService.Setup(s => s.GetUserByIdAsync(_testUserId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenOrderBelongsToDifferentUser()
        {
            // Arrange
            int orderId = 1;
            int differentUserId = _testUserId + 1;
            var order = new Order
            {
                Id = orderId,
                UserId = differentUserId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 100.00m
            };

            var user = new User { Id = _testUserId, Email = "test@example.com" };

            _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);
            _mockUserService.Setup(s => s.GetUserByIdAsync(_testUserId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrdersByUser_ReturnsOkWithUserOrders()
        {
            // Arrange
            int userId = 1;
            var expectedOrders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = 100.00m
                },
                new Order
                {
                    Id = 2,
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Processing,
                    TotalAmount = 150.00m
                }
            };

            _mockOrderService.Setup(s => s.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetOrdersByUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(expectedOrders, returnedOrders);
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtActionWithOrder()
        {
            // Arrange
            var orderDto = new CreateOrderDTO
            {
                Items = new List<CreateOrderItemDTO>
                {
                    new CreateOrderItemDTO { ProductId = 1, Quantity = 2 }
                },
                ShippingAddress = "22 John smith, London City"
            };

            var createdOrder = new Order
            {
                Id = 3,
                UserId = _testUserId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 50.00m,
                ShippingAddress = orderDto.ShippingAddress,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = 1,
                        ProductId = 1,
                        Quantity = 2,
                        Price = 25.00m
                    }
                }
            };

            _mockOrderService.Setup(s => s.CreateOrderAsync(It.Is<CreateOrderDTO>(
                dto => dto.ShippingAddress == orderDto.ShippingAddress &&
                       dto.UserId == _testUserId)))
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(OrdersController.GetOrder), createdAtActionResult.ActionName);
            Assert.Equal(createdOrder.Id, createdAtActionResult.RouteValues!["id"]);

            var returnedOrder = Assert.IsType<Order>(createdAtActionResult.Value);
            Assert.Equal(createdOrder, returnedOrder);

            // Verify that GetCurrentUserId was used to set the user ID
            _mockOrderService.Verify(s => s.CreateOrderAsync(
                It.Is<CreateOrderDTO>(dto => dto.UserId == _testUserId)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatus_ReturnsNoContent_WhenOrderExists()
        {
            // Arrange
            int orderId = 1;
            OrderStatus newStatus = OrderStatus.Shipped;
            var updatedOrder = new Order
            {
                Id = orderId,
                UserId = _testUserId,
                Status = newStatus
            };

            _mockOrderService.Setup(s => s.UpdateOrderStatusAsync(orderId, newStatus))
                .ReturnsAsync(updatedOrder);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, newStatus);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateOrderStatus_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 999;
            OrderStatus newStatus = OrderStatus.Shipped;

            _mockOrderService.Setup(s => s.UpdateOrderStatusAsync(orderId, newStatus))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, newStatus);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNoContent_WhenOrderDeleted()
        {
            // Arrange
            int orderId = 1;

            _mockOrderService.Setup(s => s.DeleteOrderAsync(orderId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNotFound_WhenOrderNotFound()
        {
            // Arrange
            int orderId = 999;

            _mockOrderService.Setup(s => s.DeleteOrderAsync(orderId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}