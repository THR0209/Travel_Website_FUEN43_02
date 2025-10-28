using Cat_Paw_Footprint.Areas.CustomerService.Repositories;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 客戶服務訊息服務層，負責訊息 ViewModel 轉換及發送者姓名/角色辨識
	/// </summary>
	public class CustomerSupportMessagesService : ICustomerSupportMessagesService
	{
		private readonly ICustomerSupportMessagesRepository _repo;
		private readonly ICustomerProfileRepository _customerRepo;
		private readonly IEmployeeMiniRepository _employeeMiniRepo;

		public CustomerSupportMessagesService(
			ICustomerSupportMessagesRepository repo,
			ICustomerProfileRepository customerRepo,
			IEmployeeMiniRepository employeeMiniRepo)
		{
			_repo = repo;
			_customerRepo = customerRepo;
			_employeeMiniRepo = employeeMiniRepo;
		}

		public async Task<IEnumerable<CustomerSupportMessageViewModel>> GetByTicketIdAsync(int ticketId)
		{
			var messages = await _repo.GetByTicketIdAsync(ticketId);
			var employeeDict = await _employeeMiniRepo.GetEmployeeNamesAsync();
			var allCustomers = await _customerRepo.GetAllAsync();
			var customerDict = allCustomers
				.Where(c => c.CustomerID.HasValue)
				.ToDictionary(c => c.CustomerID.Value, c => c.CustomerName ?? "(未知客戶)");

			return messages.Select(m => MapFromEntity(m, employeeDict, customerDict));
		}

		public async Task<IEnumerable<CustomerSupportMessageViewModel>> GetByTicketIdAsync(int ticketId, int skip, int take)
		{
			var messages = await _repo.GetByTicketIdAsync(ticketId, skip, take);
			var employeeDict = await _employeeMiniRepo.GetEmployeeNamesAsync();
			var allCustomers = await _customerRepo.GetAllAsync();
			var customerDict = allCustomers
				.Where(c => c.CustomerID.HasValue)
				.ToDictionary(c => c.CustomerID.Value, c => c.CustomerName ?? "(未知客戶)");

			return messages.Select(m => MapFromEntity(m, employeeDict, customerDict));
		}

		public async Task<CustomerSupportMessageViewModel> AddAsync(CustomerSupportMessageViewModel vm)
		{
			var entity = new CustomerSupportMessages
			{
				TicketID = vm.TicketID,
				SenderID = vm.SenderID,
				ReceiverID = vm.ReceiverID,
				MessageContent = vm.MessageContent,
				UnreadCount = vm.UnreadCount,
				AttachmentURL = vm.AttachmentURL,
				SentTime = DateTime.Now
			};

			var result = await _repo.AddAsync(entity);
			vm.MessageID = result.MessageID;
			vm.SentTime = result.SentTime;

			var employeeDict = await _employeeMiniRepo.GetEmployeeNamesAsync();
			var allCustomers = await _customerRepo.GetAllAsync();
			var customerDict = allCustomers
				.Where(c => c.CustomerID.HasValue)
				.ToDictionary(c => c.CustomerID.Value, c => c.CustomerName ?? "(未知客戶)");

			return MapToViewModel(vm, employeeDict, customerDict);
		}

		/// <summary>
		/// 根據 ViewModel 補齊 SenderRole / SenderDisplayName
		/// </summary>
		private CustomerSupportMessageViewModel MapToViewModel(
			CustomerSupportMessageViewModel vm,
			IDictionary<int, string> employeeDict,
			IDictionary<int, string> customerDict)
		{
			// ✅ 若前端有傳 SenderType，優先使用
			if (!string.IsNullOrWhiteSpace(vm.SenderType))
			{
				if (vm.SenderType.Equals("Customer", StringComparison.OrdinalIgnoreCase))
				{
					vm.SenderRole = "客戶";
					if (vm.SenderID.HasValue)
					{
						if (!customerDict.TryGetValue(vm.SenderID.Value, out var name) || string.IsNullOrWhiteSpace(name))
							name = "未知客戶";
						vm.SenderDisplayName = name;
					}
					else
					{
						vm.SenderDisplayName = vm.SenderDisplayName ?? "客戶";
					}
					return vm;
				}
				else if (vm.SenderType.Equals("Employee", StringComparison.OrdinalIgnoreCase))
				{
					vm.SenderRole = "員工";
					if (vm.SenderID.HasValue)
					{
						if (!employeeDict.TryGetValue(vm.SenderID.Value, out var name) || string.IsNullOrWhiteSpace(name))
							name = "未知員工";
						vm.SenderDisplayName = name;
					}
					else
					{
						vm.SenderDisplayName = vm.SenderDisplayName ?? "客服人員";
					}
					return vm;
				}
			}

			// ✅ 若無 SenderType，則根據 ID fallback 判斷
			if (vm.SenderID.HasValue)
			{
				var senderId = vm.SenderID.Value;

				if (employeeDict.TryGetValue(senderId, out var empName) && !string.IsNullOrWhiteSpace(empName))
				{
					vm.SenderRole = "員工";
					vm.SenderDisplayName = empName;
				}
				else if (customerDict.TryGetValue(senderId, out var cusName) && !string.IsNullOrWhiteSpace(cusName))
				{
					vm.SenderRole = "客戶";
					vm.SenderDisplayName = cusName;
				}
				else
				{
					vm.SenderRole = "未知";
					vm.SenderDisplayName = "未知";
				}
			}
			else if (vm.ReceiverID.HasValue &&
					 customerDict.TryGetValue(vm.ReceiverID.Value, out var recvName) &&
					 !string.IsNullOrWhiteSpace(recvName))
			{
				vm.SenderRole = "客戶";
				vm.SenderDisplayName = recvName;
			}
			else
			{
				vm.SenderRole = "未知";
				vm.SenderDisplayName = "未知";
			}

			return vm;
		}

		private CustomerSupportMessageViewModel MapFromEntity(
			CustomerSupportMessages entity,
			IDictionary<int, string> employeeDict,
			IDictionary<int, string> customerDict)
		{
			var vm = new CustomerSupportMessageViewModel
			{
				MessageID = entity.MessageID,
				TicketID = entity.TicketID,
				SenderID = entity.SenderID,
				ReceiverID = entity.ReceiverID,
				MessageContent = entity.MessageContent,
				UnreadCount = entity.UnreadCount,
				AttachmentURL = entity.AttachmentURL,
				SentTime = entity.SentTime
			};

			return MapToViewModel(vm, employeeDict, customerDict);
		}
	}
}
