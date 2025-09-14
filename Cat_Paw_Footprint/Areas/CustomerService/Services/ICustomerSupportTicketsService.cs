using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	public interface ICustomerSupportTicketsService
	{
		Task<IEnumerable<CustomerSupportTicketViewModel>> GetAllAsync();
		Task<CustomerSupportTicketViewModel?> GetByIdAsync(int id);
		Task AddAsync(CustomerSupportTicketViewModel vm);
		Task UpdateAsync(CustomerSupportTicketViewModel vm);
		Task DeleteAsync(int id);
		Task<bool> ExistsAsync(int id);
	}

}
