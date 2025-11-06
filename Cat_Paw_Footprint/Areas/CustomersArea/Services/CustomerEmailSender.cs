using Cat_Paw_Footprint.Areas.Order.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Cat_Paw_Footprint.Areas.CustomersArea.Services
{
	public class CustomerEmailSender: IEmailSender
	{
		private readonly SmtpOptions _opt;
		public CustomerEmailSender(IOptions<SmtpOptions> opt)
		{
			_opt = opt.Value;
		}
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                // 🟢 先使用主帳號
                _opt.UsePrimary();

                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(_opt.DisplayName, _opt.From));
                msg.To.Add(MailboxAddress.Parse(email));
                msg.Subject = subject;
                msg.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_opt.Host, _opt.Port,
                    _opt.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

                if (!string.IsNullOrWhiteSpace(_opt.User))
                    await client.AuthenticateAsync(_opt.User, _opt.Pass);

                await client.SendAsync(msg);
                await client.DisconnectAsync(true);

                Console.WriteLine($"✅ 郵件寄出成功（主帳號：{_opt.User}）");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ 主帳號寄信失敗：{ex.Message}");

                // 🔁 改用備援帳號重試
                try
                {
                    _opt.UseSecondary();

                    var msg = new MimeMessage();
                    msg.From.Add(new MailboxAddress(_opt.DisplayName, _opt.From));
                    msg.To.Add(MailboxAddress.Parse(email));
                    msg.Subject = subject;
                    msg.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

                    using var client = new SmtpClient();
                    await client.ConnectAsync(_opt.Host, _opt.Port,
                        _opt.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

                    if (!string.IsNullOrWhiteSpace(_opt.User))
                        await client.AuthenticateAsync(_opt.User, _opt.Pass);

                    await client.SendAsync(msg);
                    await client.DisconnectAsync(true);

                    Console.WriteLine($"✅ 郵件寄出成功（備援帳號：{_opt.User}）");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"❌ 備援帳號寄信失敗：{ex2.Message}");
                    throw new Exception("所有 SMTP 帳號皆無法寄信。");
                }
            }
        }

    }
}
