using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDTO orderDto)
        {
            
            await ValidateOrderItems(orderDto.Items);

            var order = new Order
            {
                TotalAmount = orderDto.TotalAmount,
                ShippingAddress = orderDto.ShippingAddress,
                UserId = orderDto.UserId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending
            };

            if (orderDto.Items != null)
            {
                foreach (var itemDto in orderDto.Items)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        Price = itemDto.Price,
                        Quantity = itemDto.Quantity,
                    };
                    order.Items.Add(orderItem);
                }
            }

            order.TotalAmount = CalculateOrderTotal(order.Items);
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            return await _orderRepository.CreateAsync(order);
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found");
            }

            ValidateStatusTransition(order.Status, status);

            order.Status = status;
            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new Exception($"Order with ID {id} not found");
            }

            // Only allow deletion of pending or cancelled orders
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    $"Cannot delete order in {order.Status} status. Only pending or cancelled orders can be deleted.");
            }

            return await _orderRepository.DeleteAsync(id);
        }

        private async Task ValidateOrderItems(ICollection<CreateOrderItemDTO> items)
        {
            if (items == null || items.Count == 0)
            {
                throw new Exception("Order must contain at least one item");
            }

            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {item.ProductId} not found");
                }

                if (product.Stock < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {item.Quantity}");
                }

                // Set the current price of the prodct as at order time
                item.Price = product.DiscountedPrice ?? product.OriginalPrice;
            }
        }

        private decimal CalculateOrderTotal(ICollection<OrderItem> items)
        {
            return items.Sum(i => i.TotalPrice);
        }

        private void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            if (currentStatus == OrderStatus.Delivered && newStatus != OrderStatus.Delivered)
            {
                throw new InvalidOperationException("Cannot change status from Delivered");
            }

            if (currentStatus == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot change status from Cancelled");
            }
        }
    }
}