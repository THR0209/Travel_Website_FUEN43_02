using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.Order.Services
{
	public interface IEmailSender
	{
		Task SendAsync(string to, string subject, string htmlBody);
	}
}