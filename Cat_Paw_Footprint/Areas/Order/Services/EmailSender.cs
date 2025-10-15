using Cat_Paw_Footprint.Areas.Order.Models;
using Cat_Paw_Footprint.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;

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
        
        public static (string Subject, string Html) BuildForOrder(CustomerOrders o)
        {
            // 基礎資料（有缺就給備援）
            var orderCode = $"ORD-{(o.CreateTime.HasValue ? o.CreateTime.Value.ToString("yyyyMMdd-HHmmss") : "NA")}-{o.OrderID}";
            var customer = o.CustomerProfile?.CustomerName ?? o.CustomerProfile?.CustomerName ?? $"客戶 {o.CustomerID}";
            var email = o.CustomerProfile?.Email ?? "";
            var productName = o.Product?.ProductName ?? $"商品 {o.ProductID}";
            var amount = (o.TotalAmount ?? 0).ToString("N0");
            var createTime = o.CreateTime?.ToString("yyyy/MM/dd HH:mm") ?? "";
            var updateTime = o.UpdateTime?.ToString("yyyy/MM/dd HH:mm") ?? "";
            var statusDesc = o.OrderStatus?.StatusDesc ?? "（未提供）";

            // 共用頁首 / 頁尾
            string Header(string title) => $@"{title}親愛的 {customer} 您好：";

        const string Footer = "如有任何問題，歡迎回信與我們聯繫。貓爪足跡 敬上";

            // 依狀態分流
            string subject;
            var sb = new StringBuilder();

            switch (statusDesc)
            {
                case "已付款":
                    subject = $"【訂單完成】{orderCode} 已付款";
                    sb.Append(Header(""));
                    sb.Append($@"
感謝您的購買！我們已收到您的款項，以下是您的訂單資訊：
訂單編號：{orderCode}
商品名稱：{productName}
應付金額：NT$ {amount}
建立時間：{createTime}
目前狀態：{statusDesc}
若您需要發票或任何協助，隨時與我們聯絡。");

                    sb.Append(Footer);
                    break;

                case "未付款":
                    subject = $"【付款提醒】{orderCode} 尚未完成付款";
                    sb.Append(Header(""));
                    sb.Append($@"
您有一筆尚未完成付款的訂單，為確保權益，請盡快完成付款：
訂單編號：{orderCode}
商品名稱：{productName}
應付金額：NT$ {amount}
建立時間：{createTime}
目前狀態：{statusDesc}
如已付款，請忽略此信或回覆告知，我們將盡快為您確認。");

                    sb.Append(Footer);
                    break;

                case "錯誤":
                    subject = $"【訂單異常】{orderCode} 需要您的協助";
                    sb.Append(Header(""));
                    sb.Append($@"
很抱歉，您的訂單在處理時發生異常，我們需要與您確認以下資訊：
訂單編號：{orderCode}
商品名稱：{productName}
應付金額：NT$ {amount}
建立時間：{createTime}
狀態：{statusDesc}
最後更新：{updateTime}
麻煩您與客服聯繫，我們將盡速協助處理。");

                    sb.Append(Footer);
                    break;

                case "已取消":
                    subject = $"【訂單取消】{orderCode} 已取消";
                    sb.Append(Header(""));
                    sb.Append($@"
您所建立的訂單已取消，以下為紀錄資訊：
訂單編號：{orderCode}
商品名稱：{productName}
應付金額：NT$ {amount}
建立時間：{createTime}
狀態：{statusDesc}
若您並未提出取消，請立即與我們聯繫。");
                    sb.Append(Footer);
                    break;

                default:
                    subject = $"【訂單通知】{orderCode}";
                    sb.Append(Header("訂單通知"));
                    sb.Append($@"
                    訂單編號：{orderCode}
                    商品名稱：{productName}
                    應付金額：NT$ {amount}
                    建立時間：{createTime}
                    狀態：{statusDesc}");
                    sb.Append(Footer);
                    break;
            }
            return (subject, sb.ToString());
        }
    }
}

