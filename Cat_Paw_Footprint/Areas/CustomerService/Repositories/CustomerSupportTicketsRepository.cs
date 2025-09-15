using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Areas.CustomerService.Repositories
{
	
	public class CustomerSupportTicketsRepository : ICustomerSupportTicketsRepository
	{
		private readonly webtravel2Context _context;

		public CustomerSupportTicketsRepository(webtravel2Context context)
		{
			_context = context;
		}

		public async Task<IEnumerable<CustomerSupportTickets>> GetAllAsync()
		{
			return await _context.CustomerSupportTickets
				.Include(t => t.Customer)
				.Include(t => t.Employee).ThenInclude(e => e.EmployeeProfile)
				.Include(t => t.Priority)
				.Include(t => t.Status)
				.Include(t => t.TicketType)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<CustomerSupportTickets?> GetByIdAsync(int id)
		{
			return await _context.CustomerSupportTickets
				.Include(t => t.Customer)
				.Include(t => t.Employee).ThenInclude(e => e.EmployeeProfile)
				.Include(t => t.Priority)
				.Include(t => t.Status)
				.Include(t => t.TicketType)
				//.AsNoTracking()
				.FirstOrDefaultAsync(t => t.TicketID == id);
		}

		public async Task AddAsync(CustomerSupportTickets ticket)
		{
			await _context.CustomerSupportTickets.AddAsync(ticket);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(CustomerSupportTickets ticket)
		{
			_context.CustomerSupportTickets.Update(ticket);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var ticket = await _context.CustomerSupportTickets.FindAsync(id);
			if (ticket != null)
			{
				_context.CustomerSupportTickets.Remove(ticket);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsAsync(int id)
		{
			return await _context.CustomerSupportTickets.AnyAsync(t => t.TicketID == id);
		}
	}
}
