using Cat_Paw_Footprint.Models;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomerService.ViewModel
{
	public class CustomerSupportTicketViewModel
	{
		[Key]
		public int TicketID { get; set; }
		public int? CustomerID { get; set; }
		public int? EmployeeID { get; set; }
		public string Subject { get; set; } = null!;
		public int? TicketTypeID { get; set; }
		public string? Description { get; set; }
		public int? StatusID { get; set; }
		public int? PriorityID { get; set; }
		public DateTime? CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		// Navigation
		public virtual CustomerProfile? Customer { get; set; }
		public virtual Employees? Employee { get; set; }
		public virtual TicketPriority? Priority { get; set; }
		public virtual TicketStatus? Status { get; set; }
		public virtual TicketTypes? TicketType { get; set; }

		// Flat fields for display
		public string? CustomerName => Customer?.CustomerName ?? "";
		public string? EmployeeName => Employee?.EmployeeProfile?.EmployeeName ?? "";
		public string? PriorityName => Priority?.PriorityDesc ?? "";
		public string? StatusName => Status?.StatusDesc ?? "";
		public string? TicketTypeName => TicketType?.TicketTypeName ?? "";
	}
}
