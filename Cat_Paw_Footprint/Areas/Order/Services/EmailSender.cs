using Cat_Paw_Footprint.Areas.Order.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.Order.Services
{
	public class EmailSender : IEmailSender
	{
		private readonly SmtpOptions _opt;
		public EmailSender(IOptions<SmtpOptions> opt) => _opt = opt.Value;

		public async Task SendAsync(string to, string subject, string htmlBody)
		{
			var msg = new MimeMessage();
			msg.From.Add(new MailboxAddress(_opt.DisplayName, _opt.From));
			msg.To.Add(MailboxAddress.Parse(to));
			msg.Subject = subject;
			msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

			using var client = new SmtpClient();
			await client.ConnectAsync(_opt.Host, _opt.Port,
				_opt.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
			if (!string.IsNullOrWhiteSpace(_opt.User))
				await client.AuthenticateAsync(_opt.User, _opt.Pass);

			await client.SendAsync(msg);
			await client.DisconnectAsync(true);
		}
	}
}
