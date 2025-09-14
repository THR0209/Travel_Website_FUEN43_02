using Cat_Paw_Footprint.Areas.CustomerService.Repositories;
using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	public class CustomerSupportFeedbackService : ICustomerSupportFeedbackService
	{
		private readonly ICustomerSupportFeedbackRepository _repo;

		public CustomerSupportFeedbackService(ICustomerSupportFeedbackRepository repo)
		{
			_repo = repo;
		}

		public Task<List<CustomerSupportFeedback>> GetAllAsync() => _repo.GetAllAsync();
		public Task<CustomerSupportFeedback?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
		public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

	}
}
