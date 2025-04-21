using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Configurations;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<IOptions<PaystackConfig>> _mockPaystackConfig;
        private readonly PaymentsController _controller;
        private readonly PaystackConfig _config;

        public PaymentControllerTests()
        {
            _mockPaymentService = new Mock<IPaymentService>();

            _config = new PaystackConfig
            {
                SecretKey = "sk_test_12345abcde",
                WebhookSecret = "webhook_secret_12345"
            };

            _mockPaystackConfig = new Mock<IOptions<PaystackConfig>>();
            _mockPaystackConfig.Setup(x => x.Value).Returns(_config);

            _controller = new PaymentsController(_mockPaymentService.Object, _mockPaystackConfig.Object);
        }

        [Fact]
        public async Task InitializePayment_ReturnsOkWithResponse_WhenSuccessful()
        {
            // Arrange
            var request = new OrderPaymentRequestDto
            {
                OrderId = 1,
                Currency = "USD"
            };

            var expectedResponse = new PaymentResponseDto
            {
                AuthorizationUrl = "https://checkout.paystack.com/testcode",
                Reference = "ref_123456789",
                AccessCode = "access_123456789"
            };

            _mockPaymentService.Setup(s => s.InitializePayment(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.InitializePayment(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<PaymentResponseDto>(okResult.Value);
            Assert.Equal(expectedResponse.AuthorizationUrl, returnedResponse.AuthorizationUrl);
            Assert.Equal(expectedResponse.Reference, returnedResponse.Reference);
            Assert.Equal(expectedResponse.AccessCode, returnedResponse.AccessCode);
        }

        [Fact]
        public async Task InitializePayment_ReturnsNotFound_WhenServiceThrowsKeyNotFoundException()
        {
            // Arrange
            var request = new OrderPaymentRequestDto
            {
                OrderId = 999, // Non-existent order
                Currency = "USD"
            };

            var exceptionMessage = "Order not found";
            _mockPaymentService.Setup(s => s.InitializePayment(request))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _controller.InitializePayment(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(exceptionMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task InitializePayment_ReturnsBadRequest_WhenServiceThrowsInvalidOperationException()
        {
            // Arrange
            var request = new OrderPaymentRequestDto
            {
                OrderId = 1,
                Currency = "XYZ" // Invalid currency
            };

            var exceptionMessage = "Invalid currency";
            _mockPaymentService.Setup(s => s.InitializePayment(request))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _controller.InitializePayment(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task VerifyPayment_ReturnsOkWithResponse_WhenSuccessful()
        {
            // Arrange
            var reference = "ref_123456789";
            var expectedResponse = new PaymentVerificationResponseDto
            {
                OrderId = 1,
                Status = "success",
                Amount = 100.00m,
                Reference = reference,
                GatewayResponse = "Successful"
            };

            _mockPaymentService.Setup(s => s.VerifyPayment(reference))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.VerifyPayment(reference);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<PaymentVerificationResponseDto>(okResult.Value);
            Assert.Equal(expectedResponse.OrderId, returnedResponse.OrderId);
            Assert.Equal(expectedResponse.Status, returnedResponse.Status);
            Assert.Equal(expectedResponse.Amount, returnedResponse.Amount);
            Assert.Equal(expectedResponse.Reference, returnedResponse.Reference);
            Assert.Equal(expectedResponse.GatewayResponse, returnedResponse.GatewayResponse);
        }

        [Fact]
        public async Task VerifyPayment_ReturnsNotFound_WhenServiceThrowsKeyNotFoundException()
        {
            // Arrange
            var reference = "invalid_reference";
            var exceptionMessage = "Payment reference not found";

            _mockPaymentService.Setup(s => s.VerifyPayment(reference))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _controller.VerifyPayment(reference);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(exceptionMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task HandleWebhook_ReturnsUnauthorized_WhenSignatureIsInvalid()
        {
            // Arrange
            var json = "{\"event\":\"charge.success\",\"data\":{\"reference\":\"ref_123456789\"}}";
            var invalidSignature = "invalid_signature";

            // Setup HttpContext with request body and headers
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            httpContext.Request.Body = stream;
            httpContext.Request.Headers["x-paystack-signature"] = invalidSignature;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _controller.HandleWebhook();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task HandleWebhook_ReturnsOk_WhenSignatureIsValidAndEventIsChargeSuccess()
        {
            // Arrange
            var payload = new PaystackWebhookPayload
            {
                Event = "charge.success",
                Data = new PaystackWebhookData
                {
                    Reference = "ref_123456789"
                }
            };
            var json = JsonSerializer.Serialize(payload);

            // Compute a valid signature using the webhook secret from config
            var signature = ComputeSignature(json, _config.WebhookSecret);

            // Setup HttpContext with request body and headers
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            httpContext.Request.Body = stream;
            httpContext.Request.Headers["x-paystack-signature"] = signature;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Setup the service to verify the payment
            _mockPaymentService.Setup(s => s.VerifyPayment(payload.Data.Reference))
                .ReturnsAsync(new PaymentVerificationResponseDto());

            // Act
            var result = await _controller.HandleWebhook();

            // Assert
            Assert.IsType<OkResult>(result);
            _mockPaymentService.Verify(s => s.VerifyPayment(payload.Data.Reference), Times.Once);
        }

        [Fact]
        public async Task HandleWebhook_ReturnsBadRequest_WhenPayloadIsInvalid()
        {
            // Arrange
            var invalidJson = "{invalid json}";

            // Compute a signature for the invalid JSON
            var signature = ComputeSignature(invalidJson, _config.WebhookSecret);

            // Setup HttpContext with request body and headers
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidJson));
            httpContext.Request.Body = stream;
            httpContext.Request.Headers["x-paystack-signature"] = signature;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _controller.HandleWebhook();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid payload", badRequestResult.Value);
        }

        // Helper method to compute a signature for testing
        private string ComputeSignature(string payload, string secret)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    // Define classes needed for webhook testing
    public class PaystackWebhookPayload
    {
        public string Event { get; set; }
        public PaystackWebhookData Data { get; set; }
    }

    public class PaystackWebhookData
    {
        public string Reference { get; set; }
    }
}