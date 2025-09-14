(() => {
    $(document).ready(function () {
        let ticketTable, dropdowns = {};
        const ticketModalEl = document.getElementById('ticketModal');
        const ticketModal = new bootstrap.Modal(ticketModalEl);

        function showLoading() {
            $('#ticketTable tbody').html('<tr><td colspan="8" class="text-center">載入中...</td></tr>');
        }

        async function loadDropdowns() {
            showLoading();
            const res = await fetch('/CustomerService/CustomerSupportTickets/GetDropdowns');
            dropdowns = await res.json();

            function fillDropdown(id, list, textField, valueField) {
                const sel = $(`#${id}`);
                sel.empty();
                list.forEach(x => sel.append(`<option value="${x[valueField] || ''}">${x[textField] || ''}</option>`));
            }

            fillDropdown('customerID', [{ customerID: '', customerName: '請選擇' }].concat(dropdowns.customers), 'customerName', 'customerID');
            fillDropdown('employeeID', [{ employeeID: '', employeeName: '請選擇' }].concat(dropdowns.employees), 'employeeName', 'employeeID');
            fillDropdown('statusID', [{ statusID: '', statusName: '請選擇' }].concat(dropdowns.statuses), 'statusName', 'statusID');
            fillDropdown('priorityID', [{ priorityID: '', priorityName: '請選擇' }].concat(dropdowns.priorities), 'priorityName', 'priorityID');
            fillDropdown('ticketTypeID', [{ ticketTypeID: '', ticketTypeName: '請選擇' }].concat(dropdowns.types), 'ticketTypeName', 'ticketTypeID');
        }

        function initTable() {
            ticketTable = $('#ticketTable').DataTable({
                ajax: {
                    url: '/CustomerService/CustomerSupportTickets/GetTickets',
                    dataSrc: ''
                },
                columns: [
                    { data: 'ticketID' },
                    { data: 'customerName', defaultContent: '' },
                    { data: 'employeeName', defaultContent: '' },
                    { data: 'statusName', defaultContent: '' },
                    { data: 'priorityName', defaultContent: '' },
                    { data: 'subject' },
                    { data: 'createTime' },
                    {
                        data: 'ticketID',
                        render: data => `
                            <button class="btn btn-secondary btn-edit" data-id="${data}">編輯</button>
                            <button class="btn btn-danger btn-delete" data-id="${data}">刪除</button>
                        `
                    }
                ],
                language: {
                    lengthMenu: "顯示 _MENU_ 筆資料",
                    zeroRecords: "無資料",
                    info: "顯示 _START_ 到 _END_ 筆資料，共 _TOTAL_ 筆",
                    infoEmpty: "顯示 0 到 0 筆資料，共 0 筆",
                    infoFiltered: "(從 _MAX_ 筆資料中篩選)",
                    search: "搜尋:",
                    paginate: { first: "第一頁", last: "最後一頁", next: "下一頁", previous: "上一頁" },
                    loadingRecords: "載入中...",
                    processing: "處理中..."
                }
            });
        }

        async function openModal(ticketID) {
            $('#ticketForm')[0].reset();
            $('#ticketID').val('');
            $('#ticketModalLabel').text(ticketID ? '編輯工單' : '新增工單');

            if (ticketID) {
                const res = await fetch(`/CustomerService/CustomerSupportTickets/GetById?id=${ticketID}`);
                if (!res.ok) {
                    alert('讀取工單失敗');
                    return;
                }

                const ticket = await res.json();
                $('#ticketID').val(ticket.ticketID || '');
                $('#customerID').val(ticket.customerID || '');
                $('#employeeID').val(ticket.employeeID || '');
                $('#subject').val(ticket.subject || '');
                $('#description').val(ticket.description || '');
                $('#ticketTypeID').val(ticket.ticketTypeID || '');
                $('#statusID').val(ticket.statusID || '');
                $('#priorityID').val(ticket.priorityID || '');
            }

            ticketModal.show();
        }

        $('#btnAddTicket').click(() => openModal(null));

        $('#ticketTable tbody').on('click', '.btn-edit', function () {
            openModal($(this).data('id'));
        });

        $('#ticketTable tbody').on('click', '.btn-delete', async function () {
            const id = $(this).data('id');
            if (!confirm('確定刪除這筆工單？')) return;

            const res = await fetch('/CustomerService/CustomerSupportTickets/DeleteTicket', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(id)
            });

            if (!res.ok) {
                alert('刪除失敗，請稍後再試');
                return;
            }

            // --- 即時刪除該列 ---
            const row = ticketTable.row($(this).parents('tr'));
            row.remove().draw(false); // false 保持分頁
        });


        $('#saveTicket').click(async function () {
            const ticketIDVal = $('#ticketID').val();
            const vm = {
                ticketID: ticketIDVal ? parseInt(ticketIDVal) : 0,
                customerID: $('#customerID').val() ? parseInt($('#customerID').val()) : null,
                employeeID: $('#employeeID').val() ? parseInt($('#employeeID').val()) : null,
                subject: $('#subject').val() || '',
                description: $('#description').val() || '',
                ticketTypeID: $('#ticketTypeID').val() ? parseInt($('#ticketTypeID').val()) : null,
                statusID: $('#statusID').val() ? parseInt($('#statusID').val()) : null,
                priorityID: $('#priorityID').val() ? parseInt($('#priorityID').val()) : null
            };

            const url = vm.ticketID > 0
                ? '/CustomerService/CustomerSupportTickets/EditTicket'
                : '/CustomerService/CustomerSupportTickets/CreateTicket';

            const res = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(vm)
            });

            if (!res.ok) {
                alert('儲存失敗，請檢查資料');
                return;
            }

            const result = await res.json();
            ticketModal.hide();

            if (vm.ticketID > 0) {
                // 編輯：即時更新該列
                const row = ticketTable.row($(`button.btn-edit[data-id="${vm.ticketID}"]`).parents('tr'));
                row.data({
                    ticketID: vm.ticketID,
                    customerName: $('#customerID option:selected').text(),
                    employeeName: $('#employeeID option:selected').text(),
                    subject: vm.subject,
                    description: vm.description,
                    ticketTypeName: $('#ticketTypeID option:selected').text(),
                    statusName: $('#statusID option:selected').text(),
                    priorityName: $('#priorityID option:selected').text(),
                    createTime: row.data().createTime
                }).draw(false);
            } else {
                // 新增：直接加到 DataTable
                const newData = {
                    ticketID: result.ticketID,
                    customerName: $('#customerID option:selected').text(),
                    employeeName: $('#employeeID option:selected').text(),
                    subject: vm.subject,
                    description: vm.description,
                    ticketTypeName: $('#ticketTypeID option:selected').text(),
                    statusName: $('#statusID option:selected').text(),
                    priorityName: $('#priorityID option:selected').text(),
                    createTime: new Date().toISOString().slice(0, 19).replace('T', ' ')
                };

                ticketTable.row.add(newData).draw(false); // 依照目前 DataTable 排序自動顯示
            }
        });


            loadDropdowns().then(initTable);
        });
})();
