using Cat_Paw_Footprint.Models;
namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	public interface ICustomerSupportFeedbackService
	{
		Task<List<CustomerSupportFeedback>> GetAllAsync();
		Task<CustomerSupportFeedback?> GetByIdAsync(int id);
		Task DeleteAsync(int id);
	}
}
