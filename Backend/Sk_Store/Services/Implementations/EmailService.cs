using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Services.Interfaces;
using System.Net.Mail;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public EmailService(IConfiguration config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
        }

        // Cách này được coi là tối ưu hơn một chút
        public Task<bool> VerifyOtpAsync(string email, string purpose, string otp)
        {
            var cacheKey = $"{purpose}_{email}";
            if (_cache.TryGetValue(cacheKey, out string? storedOtp) && storedOtp == otp)
            {
                _cache.Remove(cacheKey); // Xóa OTP sau khi xác thực thành công
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public async Task SendOtpAsync(string toEmail, string subject, string purpose)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var cacheKey = $"{purpose}_{toEmail}";

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, otp, cacheOptions);

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(
                _config["SmtpSettings:SenderName"],
                _config["SmtpSettings:SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html")
            {
                Text = $"<p>Mã xác thực của bạn là: <strong>{otp}</strong></p>" +
                       "<p>Mã này có hiệu lực trong 5 phút.</p>"
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    _config["SmtpSettings:Server"],
                    int.Parse(_config["SmtpSettings:Port"]!),
                    SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(
                    _config["SmtpSettings:Username"],
                    _config["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}