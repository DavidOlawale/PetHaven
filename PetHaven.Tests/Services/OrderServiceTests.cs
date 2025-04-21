using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using Xunit;

namespace PetHaven.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _service = new OrderService(_mockOrderRepository.Object, _mockProductRepository.Object);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsAllOrders()
        {
            // Arrange
            var expectedOrders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    UserId = 1,
                    OrderDate = DateTime.UtcNow.AddDays(-1),
                    Status = OrderStatus.Delivered,
                    TotalAmount = 100m
                },
                new Order
                {
                    Id = 2,
                    UserId = 2,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Processing,
                    TotalAmount = 150m
                }
            };

            _mockOrderRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(expectedOrders);

            // Act
            var result = await _service.GetAllOrdersAsync();

            // Assert
            Assert.Equal(expectedOrders, result);
            _mockOrderRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrder_WhenOrderExists()
        {
            // Arrange
            int orderId = 1;
            var expectedOrder = new Order
            {
                Id = orderId,
                UserId = 1,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Delivered,
                TotalAmount = 100m
            };

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _service.GetOrderByIdAsync(orderId);

            // Assert
            Assert.Equal(expectedOrder, result);
            _mockOrderRepository.Verify(r => r.GetByIdAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 999;

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _service.GetOrderByIdAsync(orderId);

            // Assert
            Assert.Null(result);
            _mockOrderRepository.Verify(r => r.GetByIdAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task GetOrdersByUserIdAsync_ReturnsUserOrders()
        {
            // Arrange
            int userId = 1;
            var expectedOrders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    UserId = userId,
                    OrderDate = DateTime.UtcNow.AddDays(-2),
                    Status = OrderStatus.Delivered,
                    TotalAmount = 75m
                },
                new Order
                {
                    Id = 3,
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = 120m
                }
            };

            _mockOrderRepository.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(expectedOrders);

            // Act
            var result = await _service.GetOrdersByUserIdAsync(userId);

            // Assert
            Assert.Equal(expectedOrders, result);
            _mockOrderRepository.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_CreatesAndReturnsOrder_WhenValid()
        {
            // Arrange
            var orderDto = new CreateOrderDTO
            {
                UserId = 1,
                ShippingAddress = "123 Test St, Test City",
                Items = new List<CreateOrderItemDTO>
                {
                    new CreateOrderItemDTO { ProductId = 1, Quantity = 2 },
                    new CreateOrderItemDTO { ProductId = 2, Quantity = 1 }
                }
            };

            var product1 = new Product
            {
                Id = 1,
                Name = "Product 1",
                OriginalPrice = 25m,
                DiscountedPrice = null,
                Stock = 10
            };

            var product2 = new Product
            {
                Id = 2,
                Name = "Product 2",
                OriginalPrice = 40m,
                DiscountedPrice = 30m,
                Stock = 5
            };

            _mockProductRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product1);
            _mockProductRepository.Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(product2);

            Order capturedOrder = null;
            var createdOrder = new Order
            {
                Id = 5,
                UserId = orderDto.UserId,
                ShippingAddress = orderDto.ShippingAddress,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 80m, // (25 * 2) + 30
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, Price = 25m },
                    new OrderItem { ProductId = 2, Quantity = 1, Price = 30m }
                }
            };

            _mockOrderRepository.Setup(r => r.CreateAsync(It.IsAny<Order>()))
                .Callback<Order>(o => capturedOrder = o)
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _service.CreateOrderAsync(orderDto);

            // Assert
            Assert.Equal(createdOrder, result);

            // Verify order was created with correct properties
            Assert.NotNull(capturedOrder);
            Assert.Equal(orderDto.UserId, capturedOrder.UserId);
            Assert.Equal(orderDto.ShippingAddress, capturedOrder.ShippingAddress);
            Assert.Equal(OrderStatus.Pending, capturedOrder.Status);

            // Verify items
            Assert.Equal(2, capturedOrder.Items.Count);
            Assert.Contains(capturedOrder.Items, i => i.ProductId == 1 && i.Quantity == 2 && i.Price == 25m);
            Assert.Contains(capturedOrder.Items, i => i.ProductId == 2 && i.Quantity == 1 && i.Price == 30m);

            // Verify total amount calculation
            Assert.Equal(80m, capturedOrder.TotalAmount);

            _mockOrderRepository.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsException_WhenNoItems()
        {
            // Arrange
            var orderDto = new CreateOrderDTO
            {
                UserId = 1,
                ShippingAddress = "123 Test St, Test City",
                Items = new List<CreateOrderItemDTO>()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderAsync(orderDto));
            Assert.Equal("Order must contain at least one item", exception.Message);

            _mockOrderRepository.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsException_WhenProductNotFound()
        {
            // Arrange
            var orderDto = new CreateOrderDTO
            {
                UserId = 1,
                ShippingAddress = "123 Test St, Test City",
                Items = new List<CreateOrderItemDTO>
                {
                    new CreateOrderItemDTO { ProductId = 999, Quantity = 2 }
                }
            };

            _mockProductRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderAsync(orderDto));
            Assert.Equal("Product with ID 999 not found", exception.Message);

            _mockOrderRepository.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsException_WhenInsufficientStock()
        {
            // Arrange
            var orderDto = new CreateOrderDTO
            {
                UserId = 1,
                ShippingAddress = "123 Test St, Test City",
                Items = new List<CreateOrderItemDTO>
                {
                    new CreateOrderItemDTO { ProductId = 1, Quantity = 20 }
                }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Product 1",
                OriginalPrice = 25m,
                Stock = 10
            };

            _mockProductRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(orderDto));
            Assert.Contains("Insufficient stock for product", exception.Message);
            Assert.Contains("Available: 10, Requested: 20", exception.Message);

            _mockOrderRepository.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_UpdatesAndReturnsOrder_WhenValidTransition()
        {
            // Arrange
            int orderId = 1;
            var orderStatus = OrderStatus.Processing;

            var existingOrder = new Order
            {
                Id = orderId,
                UserId = 1,
                Status = OrderStatus.Pending,
                TotalAmount = 100m
            };

            var updatedOrder = new Order
            {
                Id = orderId,
                UserId = 1,
                Status = orderStatus,
                TotalAmount = 100m
            };

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(existingOrder);

            _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
                .ReturnsAsync(updatedOrder);

            // Act
            var result = await _service.UpdateOrderStatusAsync(orderId, orderStatus);

            // Assert
            Assert.Equal(updatedOrder, result);
            _mockOrderRepository.Verify(r => r.GetByIdAsync(orderId), Times.Once);
            _mockOrderRepository.Verify(r => r.UpdateAsync(It.Is<Order>(o =>
                o.Id == orderId && o.Status == orderStatus)), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ThrowsException_WhenOrderNotFound()
        {
            // Arrange
            int orderId = 999;
            var orderStatus = OrderStatus.Processing;

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateOrderStatusAsync(orderId, orderStatus));
            Assert.Equal($"Order with ID {orderId} not found", exception.Message);

            _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ThrowsException_WhenInvalidTransitionFromDelivered()
        {
            // Arrange
            int orderId = 1;
            var newStatus = OrderStatus.Processing;

            var existingOrder = new Order
            {
                Id = orderId,
                UserId = 1,
                Status = OrderStatus.Delivered,
                TotalAmount = 100m
            };

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(existingOrder);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateOrderStatusAsync(orderId, newStatus));
            Assert.Equal("Cannot change status from Delivered", exception.Message);

            _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ThrowsException_WhenInvalidTransitionFromCancelled()
        {
            // Arrange
            int orderId = 1;
            var newStatus = OrderStatus.Processing;

            var existingOrder = new Order
            {
                Id = orderId,
                UserId = 1,
                Status = OrderStatus.Cancelled,
                TotalAmount = 100m
            };

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(existingOrder);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateOrderStatusAsync(orderId, newStatus));
            Assert.Equal("Cannot change status from Cancelled", exception.Message);

            _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOrderAsync_DeletesAndReturnsTrue_WhenValidStatus()
        {
            // Arrange
            int orderId = 1;

            var existingOrder = new Order
            {
                Id = orderId,
                UserId = 1,
                Status = OrderStatus.Pending,
                TotalAmount = 100m
            };

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(existingOrder);

            _mockOrderRepository.Setup(r => r.DeleteAsync(orderId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteOrderAsync(orderId);

            // Assert
            Assert.True(result);
            _mockOrderRepository.Verify(r => r.GetByIdAsync(orderId), Times.Once);
            _mockOrderRepository.Verify(r => r.DeleteAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_ThrowsException_WhenOrderNotFound()
        {
            // Arrange
            int orderId = 999;

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.DeleteOrderAsync(orderId));
            Assert.Equal($"Order with ID {orderId} not found", exception.Message);

            _mockOrderRepository.Verify(r => r.DeleteAsync(orderId), Times.Never);
        }

        [Fact]
        public async Task DeleteOrderAsync_ThrowsException_WhenInvalidStatus()
        {
            // Arrange
            int orderId = 1;

            var existingOrder = new Order
            {
                Id = orderId,
                UserId = 1,
                Status = OrderStatus.Processing,
                TotalAmount = 100m
            };

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(existingOrder);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.DeleteOrderAsync(orderId));
            Assert.Contains("Cannot delete order in Processing status", exception.Message);
            Assert.Contains("Only pending or cancelled orders can be deleted", exception.Message);

            _mockOrderRepository.Verify(r => r.DeleteAsync(orderId), Times.Never);
        }
    }
}