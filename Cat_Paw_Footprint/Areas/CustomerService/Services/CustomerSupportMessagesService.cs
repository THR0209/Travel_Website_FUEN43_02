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

		/// <summary>
		/// 透過 DI 注入各資料存取層
		/// </summary>
		public CustomerSupportMessagesService(
			ICustomerSupportMessagesRepository repo,
			ICustomerProfileRepository customerRepo,
			IEmployeeMiniRepository employeeMiniRepo)
		{
			_repo = repo;
			_customerRepo = customerRepo;
			_employeeMiniRepo = employeeMiniRepo;
		}

		/// <summary>
		/// 取得指定工單的所有訊息（不分頁），回傳 ViewModel 並自動補齊發送者姓名/角色
		/// </summary>
		public async Task<IEnumerable<CustomerSupportMessageViewModel>> GetByTicketIdAsync(int ticketId)
		{
			var messages = await _repo.GetByTicketIdAsync(ticketId);
			var employeeDict = await _employeeMiniRepo.GetEmployeeNamesAsync();

			var customerIds = messages
				.Where(m => m.SenderID.HasValue && !employeeDict.ContainsKey(m.SenderID.Value))
				.Select(m => m.SenderID.Value)
				.Distinct()
				.ToList();

			var allCustomers = await _customerRepo.GetAllAsync();
			var customerDict = allCustomers
				.Where(c => c.CustomerID.HasValue && customerIds.Contains(c.CustomerID.Value))
				.ToDictionary(c => c.CustomerID.Value, c => c.CustomerName ?? "(未知客戶)");

			return messages.Select(m => MapFromEntity(m, employeeDict, customerDict));
		}

		/// <summary>
		/// 取得指定工單的訊息（分頁），回傳 ViewModel 並自動補齊發送者姓名/角色
		/// </summary>
		public async Task<IEnumerable<CustomerSupportMessageViewModel>> GetByTicketIdAsync(int ticketId, int skip, int take)
		{
			var messages = await _repo.GetByTicketIdAsync(ticketId, skip, take);
			var employeeDict = await _employeeMiniRepo.GetEmployeeNamesAsync();

			var customerIds = messages
				.Where(m => m.SenderID.HasValue && !employeeDict.ContainsKey(m.SenderID.Value))
				.Select(m => m.SenderID.Value)
				.Distinct()
				.ToList();

			var allCustomers = await _customerRepo.GetAllAsync();
			var customerDict = allCustomers
				.Where(c => c.CustomerID.HasValue && customerIds.Contains(c.CustomerID.Value))
				.ToDictionary(c => c.CustomerID.Value, c => c.CustomerName ?? "(未知客戶)");

			return messages.Select(m => MapFromEntity(m, employeeDict, customerDict));
		}

		/// <summary>
		/// 新增訊息，補齊 ViewModel 的發送者姓名/角色
		/// </summary>
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
				SentTime = DateTime.UtcNow
			};

			var result = await _repo.AddAsync(entity);
			vm.MessageID = result.MessageID;
			vm.SentTime = result.SentTime;

			var employeeDict = await _employeeMiniRepo.GetEmployeeNamesAsync();
			var allCustomers = await _customerRepo.GetAllAsync();

			var customerDict = allCustomers
				.Where(c => vm.SenderID.HasValue && c.CustomerID == vm.SenderID.Value)
				.ToDictionary(c => c.CustomerID.Value, c => c.CustomerName ?? "(未知客戶)");

			return MapToViewModel(vm, employeeDict, customerDict);
		}

		/// <summary>
		/// 根據 ViewModel 補齊 SenderRole/SenderDisplayName
		/// </summary>
		private CustomerSupportMessageViewModel MapToViewModel(
			CustomerSupportMessageViewModel vm,
			IDictionary<int, string> employeeDict,
			IDictionary<int, string> customerDict)
		{
			if (vm.SenderID.HasValue)
			{
				var senderId = vm.SenderID.Value;
				if (employeeDict.ContainsKey(senderId))
				{
					vm.SenderRole = "員工";
					vm.SenderDisplayName = employeeDict[senderId];
				}
				else if (customerDict.ContainsKey(senderId))
				{
					vm.SenderRole = "客戶";
					vm.SenderDisplayName = customerDict[senderId];
				}
				else
				{
					vm.SenderRole = "未知";
					vm.SenderDisplayName = "未知";
				}
			}
			else
			{
				vm.SenderRole = "未知";
				vm.SenderDisplayName = "未知";
			}
			return vm;
		}

		/// <summary>
		/// 將 Entity 轉成 ViewModel 並補齊 SenderRole/SenderDisplayName
		/// </summary>
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