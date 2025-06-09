namespace Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string subject, string purpose);
        Task<bool> VerifyOtpAsync(string email, string purpose, string otp);
    }
}