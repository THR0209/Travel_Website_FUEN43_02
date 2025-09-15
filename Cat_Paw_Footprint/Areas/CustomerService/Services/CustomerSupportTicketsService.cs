using Cat_Paw_Footprint.Areas.CustomerService.Repositories;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{

	public class CustomerSupportTicketsService : ICustomerSupportTicketsService
	{
		private readonly ICustomerSupportTicketsRepository _repo;

		public CustomerSupportTicketsService(ICustomerSupportTicketsRepository repo)
		{
			_repo = repo;
		}

		public async Task<IEnumerable<CustomerSupportTicketViewModel>> GetAllAsync()
		{
			var tickets = await _repo.GetAllAsync();
			return tickets.Select(t => new CustomerSupportTicketViewModel
			{
				TicketID = t.TicketID,
				CustomerID = t.CustomerID,
				EmployeeID = t.EmployeeID,
				Subject = t.Subject,
				TicketTypeID = t.TicketTypeID,
				Description = t.Description,
				StatusID = t.StatusID,
				PriorityID = t.PriorityID,
				CreateTime = t.CreateTime,
				UpdateTime = t.UpdateTime,
				Customer = t.Customer,
				Employee = t.Employee,
				Priority = t.Priority,
				Status = t.Status,
				TicketType = t.TicketType
			});
		}

		public async Task<CustomerSupportTicketViewModel?> GetByIdAsync(int id)
		{
			var t = await _repo.GetByIdAsync(id);
			if (t == null) return null;
			return new CustomerSupportTicketViewModel
			{
				TicketID = t.TicketID,
				CustomerID = t.CustomerID,
				EmployeeID = t.EmployeeID,
				Subject = t.Subject,
				TicketTypeID = t.TicketTypeID,
				Description = t.Description,
				StatusID = t.StatusID,
				PriorityID = t.PriorityID,
				CreateTime = t.CreateTime,
				UpdateTime = t.UpdateTime,
				Customer = t.Customer,
				Employee = t.Employee,
				Priority = t.Priority,
				Status = t.Status,
				TicketType = t.TicketType
			};
		}

		public async Task AddAsync(CustomerSupportTicketViewModel vm)
		{
			var entity = new CustomerSupportTickets
			{
				CustomerID = vm.CustomerID,
				EmployeeID = vm.EmployeeID,
				Subject = vm.Subject,
				TicketTypeID = vm.TicketTypeID,
				Description = vm.Description,
				StatusID = vm.StatusID,
				PriorityID = vm.PriorityID,
				CreateTime = DateTime.Now,
				UpdateTime = DateTime.Now
			};
			await _repo.AddAsync(entity);
		}

		public async Task UpdateAsync(CustomerSupportTicketViewModel vm)
		{
			var entity = await _repo.GetByIdAsync(vm.TicketID);
			if (entity == null) return;
			entity.CustomerID = vm.CustomerID;
			entity.EmployeeID = vm.EmployeeID;
			entity.Subject = vm.Subject;
			entity.TicketTypeID = vm.TicketTypeID;
			entity.Description = vm.Description;
			entity.StatusID = vm.StatusID;
			entity.PriorityID = vm.PriorityID;
			entity.UpdateTime = DateTime.Now;
			await _repo.UpdateAsync(entity);
		}

		public async Task DeleteAsync(int id)
		{
			await _repo.DeleteAsync(id);
		}

		public async Task<bool> ExistsAsync(int id)
		{
			return await _repo.ExistsAsync(id);
		}
	}
}
