using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	public interface ICustomerSupportTicketsRepository
	{
		Task<IEnumerable<CustomerSupportTickets>> GetAllAsync();
		Task<CustomerSupportTickets?> GetByIdAsync(int id);
		Task AddAsync(CustomerSupportTickets ticket);
		Task UpdateAsync(CustomerSupportTickets ticket);
		Task DeleteAsync(int id);
		Task<bool> ExistsAsync(int id);
	}
}
