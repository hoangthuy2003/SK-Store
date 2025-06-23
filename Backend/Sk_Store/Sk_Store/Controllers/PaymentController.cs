using Microsoft.AspNetCore.Mvc;
using Repositories.UnitOfWork;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Collections.Generic; // <<< THÊM USING NÀY
using System.Linq; // <<< THÊM USING NÀY

namespace Sk_Store.Controllers
{
    public class PayOsResponseDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("desc")]
        public string Desc { get; set; }

        [JsonPropertyName("data")]
        public JsonElement? Data { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(
            IConfiguration config,
            IUnitOfWork unitOfWork,
            IHttpClientFactory httpClientFactory,
            ILogger<PaymentController> logger) // <<< INJECT LOGGER VÀO CONSTRUCTOR
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
            _logger = logger; // <<< GÁN LOGGER
        }

        [HttpPost("payos/{orderId}")]
        public async Task<IActionResult> CreatePayOsPaymentUrl(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            var payOsConfig = _config.GetSection("PayOsSettings");
            var returnUrl = "http://localhost:4200/orders/" + orderId;
            var cancelUrl = "http://localhost:4200/checkout";

            // <<< SỬA LẠI CÁCH TẠO DỮ LIỆU VÀ CHỮ KÝ >>>
            var paymentData = new Dictionary<string, object>
            {
                { "orderCode", order.OrderId }, // PayOS yêu cầu orderCode là số nguyên
                { "amount", (int)order.TotalAmount },
                { "description", $"Thanh toan don hang #{order.OrderId}" },
                { "returnUrl", returnUrl },
                { "cancelUrl", cancelUrl }
            };

            var signature = CreateSignature(paymentData, payOsConfig["ChecksumKey"]);

            var requestPayload = new
            {
                orderCode = order.OrderId,
                amount = (int)order.TotalAmount,
                description = $"Thanh toan don hang #{order.OrderId}",
                returnUrl,
                cancelUrl,
                signature
            };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-client-id", payOsConfig["ClientId"]);
            client.DefaultRequestHeaders.Add("x-api-key", payOsConfig["ApiKey"]);

            var response = await client.PostAsJsonAsync($"{payOsConfig["ApiBaseUrl"]}/v2/payment-requests", requestPayload);

            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { message = "Lỗi khi giao tiếp với PayOS.", details = responseString });
            }

            var payOsResponse = JsonSerializer.Deserialize<PayOsResponseDto>(responseString);

            if (payOsResponse.Code != "00")
            {
                return BadRequest(new { message = $"Lỗi từ PayOS: {payOsResponse.Desc}" });
            }

            return Ok(payOsResponse);
        }

        [HttpPost("payos-webhook")]
        public async Task<IActionResult> PayOsWebhook([FromBody] JsonElement body)
        {
            // <<< THÊM LOGGING ĐỂ DEBUG >>>
            _logger.LogInformation("Received PayOS Webhook: {WebhookBody}", body.ToString());

            // Kiểm tra xem webhook có thuộc tính 'data' không
            if (!body.TryGetProperty("data", out var data))
            {
                _logger.LogWarning("PayOS Webhook received without 'data' property.");
                return Ok();
            }

            var code = body.TryGetProperty("code", out var codeElement) ? codeElement.GetString() : null;
            if (code != "00")
            {
                _logger.LogInformation("Webhook ignored because code is not '00'. Code: {Code}", code);
                return Ok(); // Bỏ qua các webhook không phải là thanh toán thành công
            }

            if (!data.TryGetProperty("orderCode", out var orderCodeElement))
            {
                _logger.LogWarning("Webhook 'data' property does not contain 'orderCode'.");
                return Ok();
            }

            var orderCode = orderCodeElement.GetInt32();

            var order = await _unitOfWork.Orders.GetByIdAsync(orderCode);
            if (order != null)
            {
                if (order.PaymentStatus != "Paid")
                {
                    order.PaymentStatus = "Paid";
                    await _unitOfWork.CompleteAsync();
                    _logger.LogInformation("Order {OrderCode} payment status updated to Paid.", orderCode);
                }
                else
                {
                    _logger.LogInformation("Order {OrderCode} was already marked as Paid.", orderCode);
                }
            }
            else
            {
                _logger.LogWarning("Webhook received for a non-existent order. OrderCode: {OrderCode}", orderCode);
            }

            return Ok();
        }

        // <<< SỬA LẠI HÀM TẠO CHỮ KÝ >>>
        private string CreateSignature(Dictionary<string, object> data, string secretKey)
        {
            // Sắp xếp các key theo thứ tự bảng chữ cái
            var sortedData = data.OrderBy(kvp => kvp.Key, StringComparer.Ordinal);

            // Tạo chuỗi dữ liệu để ký
            var dataToSign = string.Join("&", sortedData.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}