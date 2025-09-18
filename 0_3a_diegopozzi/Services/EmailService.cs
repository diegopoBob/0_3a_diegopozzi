using MailKit.Net.Smtp;
using MimeKit;

namespace _0_3a_diegopozzi.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string verificationUrl)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Soporte Demo", _config["Smtp:User"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Verificación de correo";

            message.Body = new TextPart("plain")
            {
                Text = $"Hacé clic en el siguiente enlace para verificar tu cuenta:\n{verificationUrl}"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]), false);
            await client.AuthenticateAsync(_config["Smtp:User"], _config["Smtp:Pass"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
