using Core.Interfaces;
using Microsoft.Extensions.Options;
using Models.Configs;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class EmailCore : IEmailService
    {

        private readonly EmailConfig _emailConfig;
        private readonly ILogger<EmailCore> _logger;
        public EmailCore(IOptions<EmailConfig> emailConfig, ILogger<EmailCore> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
        }
        public async Task<bool> SendLowStockEmail(List<IngredientStockDTO> ingredients)
        {
            var smtpClient = new SmtpClient(_emailConfig.Smtp.Server)
            {
                Port = _emailConfig.Smtp.Port,
                Credentials = new NetworkCredential(_emailConfig.Smtp.Username, _emailConfig.Smtp.Password),
                EnableSsl = _emailConfig.Smtp.EnableSsl,
            };

            string ingredientsName = string.Join(", ", ingredients.Select(x => x.Ingredient.IngredientName).ToList());

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailConfig.From),
                Subject = "Low Stock Alert",
                Body = $"The stock level for {ingredientsName} have reached below 50%. Please restock as soon as possible.",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(_emailConfig.To);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
