using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SportShop.Models;

namespace SportShop.Services;

public class OrderEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderEmailService> _logger;

    public OrderEmailService(
        IConfiguration configuration,
        ILogger<OrderEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(
        string toEmail,
        string customerName,
        string address,
        string phone,
        IEnumerable<CartItem> items,
        decimal total)
    {
        var host = _configuration["MailSettings:Host"];
        var portText = _configuration["MailSettings:Port"];
        var username = _configuration["MailSettings:Username"];
        var password = _configuration["MailSettings:Password"];
        var fromEmail = _configuration["MailSettings:FromEmail"];
        var fromName = _configuration["MailSettings:FromName"] ?? "SportShop";
        var adminCopyTo = _configuration["MailSettings:AdminCopyTo"]; // optional

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(portText) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException(
                "Mail settings are missing. Check MailSettings in appsettings.json.");
        }

        if (!int.TryParse(portText, out var port))
        {
            throw new InvalidOperationException("MailSettings:Port must be a valid number.");
        }

        var subject = $"SportShop - Order confirmation - {DateTime.Now:yyyy-MM-dd HH:mm}";
        var body = BuildOrderEmailBody(customerName, address, phone, items, total);

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        message.To.Add(toEmail);

        if (!string.IsNullOrWhiteSpace(adminCopyTo))
        {
            message.Bcc.Add(adminCopyTo);
        }

        using var smtp = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 20000
        };

        try
        {
            await smtp.SendMailAsync(message);

            _logger.LogInformation(
                "Order confirmation email sent successfully to {Email}",
                toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send order confirmation email to {Email}",
                toEmail);

            throw;
        }
    }

    private static string BuildOrderEmailBody(
        string customerName,
        string address,
        string phone,
        IEnumerable<CartItem> items,
        decimal total)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Hello {customerName},");
        sb.AppendLine();
        sb.AppendLine("Thank you for your order from SportShop.");
        sb.AppendLine("Here are your order details:");
        sb.AppendLine();

        sb.AppendLine($"Customer: {customerName}");
        sb.AppendLine($"Phone: {phone}");
        sb.AppendLine($"Address: {address}");
        sb.AppendLine();

        sb.AppendLine("Items:");
        sb.AppendLine("-------------------------------------");

        foreach (var item in items)
        {
            var productName = item.Product?.Name ?? "Unknown product";
            var size = string.IsNullOrWhiteSpace(item.SelectedSize) ? "N/A" : item.SelectedSize;
            var quantity = item.Quantity;
            var price = item.Product?.Price ?? 0m;
            var lineTotal = price * quantity;

            sb.AppendLine($"Product: {productName}");
            sb.AppendLine($"Size: {size}");
            sb.AppendLine($"Quantity: {quantity}");
            sb.AppendLine($"Unit price: {price:0.00} EUR");
            sb.AppendLine($"Line total: {lineTotal:0.00} EUR");
            sb.AppendLine("-------------------------------------");
        }

        sb.AppendLine();
        sb.AppendLine($"Order total: {total:0.00} EUR");
        sb.AppendLine();
        sb.AppendLine("We will process your order soon.");
        sb.AppendLine();
        sb.AppendLine("SportShop");

        return sb.ToString();
    }
}