//using Microsoft.Extensions.Configuration;
//using Moq;
//using Paystack.Net.SDK;
//using Paystack.Net.SDK.Models;
//using Paystack.Net.SDK.Transactions;
//using PetHaven.BusinessLogic.DTOs;
//using PetHaven.BusinessLogic.Services;
//using PetHaven.Data.Model;
//using PetHaven.Data.Repositories.Interfaces;
//using System;
//using System.Threading.Tasks;
//using Xunit;

//namespace PetHaven.Tests.Services
//{
//    public class PaystackPaymentServiceTests
//    {
//        private readonly Mock<IPaymentRepository> _mockPaymentRepository;
//        private readonly Mock<IOrderRepository> _mockOrderRepository;
//        private readonly Mock<IConfiguration> _mockConfiguration;
//        private readonly Mock<PayStackApi> _mockPaystackApi;
//        private readonly Mock<ITransactions> _mockTransactions;
//        private readonly PaystackPaymentService _service;

//        public PaystackPaymentServiceTests()
//        {
//            _mockPaymentRepository = new Mock<IPaymentRepository>();
//            _mockOrderRepository = new Mock<IOrderRepository>();
//            _mockConfiguration = new Mock<IConfiguration>();
//            _mockPaystackApi = new Mock<PayStackApi>("test-key");
//            _mockTransactions = new Mock<ITransactions>();

//            _mockConfiguration.Setup(c => c["Paystack:SecretKey"]).Returns("test-secret-key");
//            _mockConfiguration.Setup(c => c["BaseUrl"]).Returns("https://test.pethaven.com");

//            // Setup mock PaystackApi and its Transactions property
//            _mockPaystackApi.Setup(p => p.Transactions).Returns(_mockTransactions.Object);

//            // Create a test service that uses the mocked PaystackApi
//            _service = new TestablePaystackPaymentService(
//                _mockPaymentRepository.Object,
//                _mockOrderRepository.Object,
//                _mockConfiguration.Object,
//                _mockPaystackApi.Object);
//        }

//        [Fact]
//        public async Task InitializePayment_ReturnsResponse_WhenOrderExists()
//        {
//            // Arrange
//            var request = new OrderPaymentRequestDto
//            {
//                OrderId = 1,
//                Currency = "USD"
//            };

//            var order = new Order
//            {
//                Id = request.OrderId,
//                TotalAmount = 100.00m,
//                Status = OrderStatus.Pending,
//                User = new User { Email = "test@example.com" }
//            };

//            _mockOrderRepository.Setup(r => r.GetByIdAsync(request.OrderId))
//                .ReturnsAsync(order);

//            Payment capturedPayment = null;
//            _mockPaymentRepository.Setup(r => r.AddAsync(It.IsAny<Payment>()))
//                .Callback<Payment>(p => capturedPayment = p)
//                .Returns(Task.CompletedTask);

//            var paystackResponse = new TransactionInitializationResponseModel
//            {
//                status = true,
//                message = "Authorization URL created",
//                data = new TransactionInitializationResponseData
//                {
//                    authorization_url = "https://checkout.paystack.com/test-auth-url",
//                    access_code = "access_code_test",
//                    reference = "ref_test"
//                }
//            };

//            _mockTransactions.Setup(t => t.InitializeTransaction(It.IsAny<TransactionInitializationRequestModel>()))
//                .ReturnsAsync(paystackResponse);

//            // Act
//            var result = await _service.InitializePayment(request);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(paystackResponse.data.authorization_url, result.AuthorizationUrl);
//            Assert.NotNull(result.Reference);

//            // Verify payment was created with correct properties
//            Assert.NotNull(capturedPayment);
//            Assert.Equal(request.OrderId, capturedPayment.OrderId);
//            Assert.Equal(order.TotalAmount, capturedPayment.Amount);
//            Assert.Equal(request.Currency, capturedPayment.Currency);
//            Assert.Equal(PaymentStatus.Pending, capturedPayment.Status);

//            _mockPaymentRepository.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
//        }

//        [Fact]
//        public async Task InitializePayment_ThrowsException_WhenOrderNotFound()
//        {
//            // Arrange
//            var request = new OrderPaymentRequestDto
//            {
//                OrderId = 999,
//                Currency = "USD"
//            };

//            _mockOrderRepository.Setup(r => r.GetByIdAsync(request.OrderId))
//                .ReturnsAsync((Order)null);

//            // Act & Assert
//            var exception = await Assert.ThrowsAsync<Exception>(() => _service.InitializePayment(request));
//            Assert.Equal("Order not found", exception.Message);

//            _mockPaymentRepository.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Never);
//        }

//        [Fact]
//        public async Task InitializePayment_ThrowsException_WhenOrderNotPending()
//        {
//            // Arrange
//            var request = new OrderPaymentRequestDto
//            {
//                OrderId = 1,
//                Currency = "USD"
//            };

//            var order = new Order
//            {
//                Id = request.OrderId,
//                TotalAmount = 100.00m,
//                Status = OrderStatus.Processing // Not Pending
//            };

//            _mockOrderRepository.Setup(r => r.GetByIdAsync(request.OrderId))
//                .ReturnsAsync(order);

//            // Act & Assert
//            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.InitializePayment(request));
//            Assert.Equal("Order payment already initialized", exception.Message);

//            _mockPaymentRepository.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Never);
//        }

//        [Fact]
//        public async Task VerifyPayment_ReturnsSuccessfulStatus_WhenPaymentSucceeds()
//        {
//            // Arrange
//            string reference = "test-reference";
//            var payment = new Payment
//            {
//                Id = 1,
//                OrderId = 1,
//                Reference = reference,
//                Amount = 100.00m,
//                Status = PaymentStatus.Pending,
//                Order = new Order
//                {
//                    Id = 1,
//                    Status = OrderStatus.Pending
//                }
//            };

//            _mockPaymentRepository.Setup(r => r.GetByReferenceAsync(reference, true))
//                .ReturnsAsync(payment);

//            var verifyResponse = new TransactionVerifyResponseModel
//            {
//                status = true,
//                data = new TransactionVerifyResponseData
//                {
//                    status = "success",
//                    gateway_response = "Successful",
//                    reference = reference
//                }
//            };

//            _mockTransactions.Setup(t => t.VerifyTransaction(reference))
//                .ReturnsAsync(verifyResponse);

//            // Act
//            var result = await _service.VerifyPayment(reference);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(payment.OrderId, result.OrderId);
//            Assert.Equal("successful", result.Status);
//            Assert.Equal(payment.Amount, result.Amount);
//            Assert.Equal(payment.Reference, result.Reference);
//            Assert.Equal("Successful", result.GatewayResponse);

//            // Verify payment was updated
//            Assert.Equal(PaymentStatus.Successful, payment.Status);
//            Assert.Equal("Successful", payment.GatewayResponse);
//            Assert.NotNull(payment.VerifiedAt);

//            // Verify order status was updated
//            Assert.Equal(OrderStatus.Processing, payment.Order.Status);

//            _mockPaymentRepository.Verify(r => r.UpdateAsync(payment), Times.Once);
//            _mockOrderRepository.Verify(r => r.UpdateAsync(payment.Order), Times.Once);
//        }

//        [Fact]
//        public async Task VerifyPayment_ReturnsFailedStatus_WhenPaymentFails()
//        {
//            // Arrange
//            string reference = "test-reference";
//            var payment = new Payment
//            {
//                Id = 1,
//                OrderId = 1,
//                Reference = reference,
//                Amount = 100.00m,
//                Status = PaymentStatus.Pending,
//                Order = new Order
//                {
//                    Id = 1,
//                    Status = OrderStatus.Pending
//                }
//            };

//            _mockPaymentRepository.Setup(r => r.GetByReferenceAsync(reference, true))
//                .ReturnsAsync(payment);

//            var verifyResponse = new TransactionVerifyResponseModel
//            {
//                status = true,
//                data = new TransactionVerifyResponseData
//                {
//                    status = "failed",
//                    gateway_response = "Payment Failed",
//                    reference = reference
//                }
//            };

//            _mockTransactions.Setup(t => t.VerifyTransaction(reference))
//                .ReturnsAsync(verifyResponse);

//            // Act
//            var result = await _service.VerifyPayment(reference);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(payment.OrderId, result.OrderId);
//            Assert.Equal("failed", result.Status);
//            Assert.Equal(payment.Amount, result.Amount);
//            Assert.Equal(payment.Reference, result.Reference);
//            Assert.Equal("Payment Failed", result.GatewayResponse);

//            // Verify payment was updated
//            Assert.Equal(PaymentStatus.Failed, payment.Status);
//            Assert.Equal("Payment Failed", payment.GatewayResponse);
//            Assert.NotNull(payment.VerifiedAt);

//            // Verify order status was NOT updated
//            Assert.Equal(OrderStatus.Pending, payment.Order.Status);

//            _mockPaymentRepository.Verify(r => r.UpdateAsync(payment), Times.Once);
//            _mockOrderRepository.Verify(r => r.UpdateAsync(payment.Order), Times.Never);
//        }

//        [Fact]
//        public async Task VerifyPayment_ReturnsExistingStatus_WhenPaymentAlreadyVerified()
//        {
//            // Arrange
//            string reference = "test-reference";
//            var payment = new Payment
//            {
//                Id = 1,
//                OrderId = 1,
//                Reference = reference,
//                Amount = 100.00m,
//                Status = PaymentStatus.Successful, // Already verified
//                GatewayResponse = "Successful",
//                Order = new Order
//                {
//                    Id = 1,
//                    Status = OrderStatus.Processing
//                }
//            };

//            _mockPaymentRepository.Setup(r => r.GetByReferenceAsync(reference, true))
//                .ReturnsAsync(payment);

//            // Act
//            var result = await _service.VerifyPayment(reference);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(payment.OrderId, result.OrderId);
//            Assert.Equal("successful", result.Status);
//            Assert.Equal(payment.Amount, result.Amount);
//            Assert.Equal(payment.Reference, result.Reference);
//            Assert.Equal("Successful", result.GatewayResponse);

//            // Verify no updates were made
//            _mockTransactions.Verify(t => t.VerifyTransaction(It.IsAny<string>()), Times.Never);
//            _mockPaymentRepository.Verify(r => r.UpdateAsync(It.IsAny<Payment>()), Times.Never);
//            _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
//        }

//        [Fact]
//        public async Task VerifyPayment_ThrowsException_WhenPaymentNotFound()
//        {
//            // Arrange
//            string reference = "nonexistent-reference";

//            _mockPaymentRepository.Setup(r => r.GetByReferenceAsync(reference, true))
//                .ReturnsAsync((Payment)null);

//            // Act & Assert
//            var exception = await Assert.ThrowsAsync<Exception>(() => _service.VerifyPayment(reference));
//            Assert.Equal("Payment not found", exception.Message);
//        }
//    }

//    // A testable version of the PaystackPaymentService that allows injecting a mock PayStackApi
//    public class TestablePaystackPaymentService : PaystackPaymentService
//    {
//        private readonly PayStackApi _customPaystackApi;

//        public TestablePaystackPaymentService(
//            IPaymentRepository paymentRepo,
//            IOrderRepository orderRepo,
//            IConfiguration config,
//            PayStackApi paystackApi)
//            : base(paymentRepo, orderRepo, config)
//        {
//            _customPaystackApi = paystackApi;
//        }

//        public PayStackApi CreatePaystackApi()
//        {
//            return _customPaystackApi;
//        }
//    }
//}