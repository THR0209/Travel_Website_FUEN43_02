using Cat_Paw_Footprint.Models;
namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	public interface ICustomerSupportFeedbackRepository
	{
		Task<List<CustomerSupportFeedback>> GetAllAsync();
		Task<CustomerSupportFeedback?> GetByIdAsync(int id);
		Task DeleteAsync(int id);
	}
}
