using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Paystack.Net.SDK.Transactions;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Configurations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly PaystackConfig _paystackConfig;

    public PaymentsController(
        IPaymentService paymentService,
        IOptions<PaystackConfig> paystackConfig)
    {
        _paymentService = paymentService;
        _paystackConfig = paystackConfig.Value;
    }

    [HttpPost("initialize")]
    [ProducesResponseType(typeof(PaymentResponseDto), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> InitializePayment([FromBody] OrderPaymentRequestDto request)
    {
        try
        {
            var response = await _paymentService.InitializePayment(request);
            return Ok(response); 
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("verify/{reference}")]
    [ProducesResponseType(typeof(PaymentVerificationResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> VerifyPayment(string reference)
    {
        try
        {
            var response = await _paymentService.VerifyPayment(reference);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        // 1. Read the request body
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["x-paystack-signature"];

        // 2. Verify the signature
        if (!VerifyPaystackSignature(json, signature))
        {
            return Unauthorized();
        }

        // 3. Process the webhook
        try
        {
            var payload = JsonSerializer.Deserialize<PaystackWebhookPayload>(json);
            if (payload?.Event == "charge.success")
            {
                await _paymentService.VerifyPayment(payload.Data.Reference);
            }
            return Ok();
        }
        catch (JsonException)
        {
            return BadRequest("Invalid payload");
        }
    }

    private bool VerifyPaystackSignature(string payload, string signature)
    {
        if (string.IsNullOrEmpty(_paystackConfig.WebhookSecret))
            return false;

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_paystackConfig.WebhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

        return signature == computedSignature;
    }
}