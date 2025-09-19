(() => {
    $(document).ready(function () {
        let ticketTable, dropdowns = {};
        const ticketModalEl = document.getElementById('ticketModal');
        const ticketModal = new bootstrap.Modal(ticketModalEl, { focus: false });

        // 顯示載入中
        function showLoading() {
            $('#ticketTable tbody').html('<tr><td colspan="8" class="text-center">載入中...</td></tr>');
        }

        // 載入下拉選單
        async function loadDropdowns() {
            showLoading();
            const res = await fetch('/CustomerService/CustomerSupportTickets/GetDropdowns');
            dropdowns = await res.json();

            function fillDropdown(id, list, textField, valueField) {
                const sel = $(`#${id}`);
                sel.empty();
                list.forEach(x => sel.append(`<option value="${x[valueField] || ''}">${x[textField] || ''}</option>`));
            }

            fillDropdown('statusID', [{ statusID: '', statusName: '請選擇' }].concat(dropdowns.statuses), 'statusName', 'statusID');
            fillDropdown('priorityID', [{ priorityID: '', priorityName: '請選擇' }].concat(dropdowns.priorities), 'priorityName', 'priorityID');
            fillDropdown('ticketTypeID', [{ ticketTypeID: '', ticketTypeName: '請選擇' }].concat(dropdowns.types), 'ticketTypeName', 'ticketTypeID');
        }

        // 初始化 DataTable
        function initTable() {
            ticketTable = $('#ticketTable').DataTable({
                ajax: {
                    url: '/CustomerService/CustomerSupportTickets/GetTickets',
                    dataSrc: ''
                },
                columns: [
                    { data: 'ticketCode' },
                    { data: 'customerName', defaultContent: '' },
                    { data: 'employeeName', defaultContent: '' },
                    { data: 'statusName', defaultContent: '' },
                    { data: 'priorityName', defaultContent: '' },
                    { data: 'subject' },
                    { data: 'createTime' },
                    {
                        data: 'ticketID',
                        render: function (data) {
                            if (typeof data === "undefined" || data === null) return '';
                            return '<button class="btn btn-primary btn-edit" data-id="' + data + '">檢視</button>' +
                                '<button class="btn btn-danger btn-delete" data-id="' + data + '">刪除</button>';
                        }
                    }
                ],
                order: [[6, 'asc']],
                rowCallback: function (row, data) {
                    if (data.statusID === 1) $(row).addClass('table-warning');
                },
                language: {
                    lengthMenu: "顯示 _MENU_ 筆資料",
                    zeroRecords: "無資料",
                    info: "顯示 _START_ 到 _END_ 筆資料，共 _TOTAL_ 筆",
                    infoEmpty: "顯示 0 到 0 筆資料，共 0 筆",
                    infoFiltered: "(從 _MAX_ 筆資料中篩選)",
                    search: "搜尋:",
                    paginate: {
                        first: "第一頁",
                        last: "最後一頁",
                        next: "下一頁",
                        previous: "上一頁"
                    },
                    loadingRecords: "載入中...",
                    processing: "處理中..."
                }
            });
        }

        // 初始化客戶 autocomplete
        function initCustomerAutocomplete() {
            $("#customerName").autocomplete({
                source: async function (request, response) {
                    const term = request.term || "";
                    const res = await fetch(`/CustomerService/CustomerSupportTickets/GetCustomersAutocomplete?term=${term}`);
                    const data = await res.json();
                    response(data);
                },
                minLength: 0,
                appendTo: "#ticketModal",
                autoFocus: true,
                select: function (event, ui) {
                    event.preventDefault();
                    $("#customerName").val(ui.item.label);
                    $("#customerID").val(ui.item.value);
                }
            }).focus(function () {
                $(this).autocomplete("search", "");
            });

            $(document).on("mousedown", ".ui-menu-item", function (e) {
                e.preventDefault();
            });
        }

        // 開啟 Modal
        async function openModal(ticketID) {
            $('#ticketForm')[0].reset();

            // 取得目前登入員工資訊
            const currentEmpName = $('#employeeName').attr('data-empname') || $('#employeeName').val();
            const currentEmpID = $('#employeeID').val();

            // 統一：新增/編輯都顯示目前登入者
            $('#employeeName').val(currentEmpName).prop('disabled', true);
            $('#employeeID').val(currentEmpID);

            if (ticketID) {
                if (typeof ticketID === "undefined" || ticketID === null || isNaN(ticketID)) {
                    alert("工單ID無效，無法編輯");
                    return;
                }

                const res = await fetch(`/CustomerService/CustomerSupportTickets/GetById?id=${ticketID}`);
                if (!res.ok) {
                    alert("找不到工單資料");
                    return;
                }

                let ticket;
                try {
                    ticket = await res.json();
                } catch (e) {
                    alert("工單資料解析失敗");
                    return;
                }

                $('#ticketID').val(ticket.ticketID);
                $('#customerName').val(ticket.customerName).prop('disabled', true);
                $('#customerID').val(ticket.customerID);
                // 處理人員顯示目前登入者（已在上面處理）
                $('#subject').val(ticket.subject).prop('disabled', true);
                $('#description').val(ticket.description).prop('disabled', true);
                $('#ticketTypeID').val(ticket.ticketTypeID).removeAttr('disabled');
                $('#statusID').val(ticket.statusID).removeAttr('disabled');
                $('#priorityID').val(ticket.priorityID).prop('disabled', true);

            } else {
                $('#ticketID').val('');
                $('#customerName').prop('disabled', false).val('');
                $('#customerID').val('');
                $('#employeeName').prop('disabled', true).val(currentEmpName);
                $('#employeeID').val(currentEmpID);
                $('#subject').prop('disabled', false).val('');
                $('#description').prop('disabled', false).val('');
                $('#ticketTypeID').prop('disabled', false).val('');
                $('#statusID').prop('disabled', false).val('');
                $('#priorityID').prop('disabled', false).val('');

                if ($("#customerName").data("ui-autocomplete")) {
                    $("#customerName").autocomplete("destroy");
                }
                initCustomerAutocomplete();
            }

            ticketModal.show();
        }

        // 初始化
        loadDropdowns().then(() => {
            initTable();

            // 新增工單
            $('#btnAddTicket').click(() => openModal(null));

            // 編輯工單
            $('#ticketTable tbody').on('click', '.btn-edit', function () {
                const id = $(this).data('id');
                openModal(id);
            });

            // 刪除工單
            $('#ticketTable tbody').on('click', '.btn-delete', async function () {
                const id = $(this).data('id');
                if (!confirm('確定刪除這筆工單？')) return;

                const res = await fetch('/CustomerService/CustomerSupportTickets/DeleteTicket', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(id)
                });

                if (!res.ok) return alert('刪除失敗');
                ticketTable.row($(this).closest('tr')).remove().draw(false);
            });

            // 儲存工單
            $('#saveTicket').click(async function () {
                $('#saveTicket').prop('disabled', true);

                if (!$('#customerID').val()) { alert('請選擇客戶'); $('#saveTicket').prop('disabled', false); return; }
                if (!$('#employeeID').val()) { alert('系統錯誤：找不到處理人員'); $('#saveTicket').prop('disabled', false); return; }
                if (!$('#subject').val()) { alert('請輸入主旨'); $('#saveTicket').prop('disabled', false); return; }
                if (!$('#ticketTypeID').val()) { alert('請選擇工單類型'); $('#saveTicket').prop('disabled', false); return; }
                if (!$('#statusID').val()) { alert('請選擇狀態'); $('#saveTicket').prop('disabled', false); return; }
                if (!$('#priorityID').val()) { alert('請選擇優先度'); $('#saveTicket').prop('disabled', false); return; }

                const ticketIDVal = $('#ticketID').val();
                const vm = {
                    ticketID: ticketIDVal ? parseInt(ticketIDVal) : 0,
                    customerID: parseInt($('#customerID').val()),
                    employeeID: parseInt($('#employeeID').val()),
                    subject: $('#subject').val(),
                    description: $('#description').val(),
                    ticketTypeID: parseInt($('#ticketTypeID').val()),
                    statusID: parseInt($('#statusID').val()),
                    priorityID: parseInt($('#priorityID').val())
                };

                const url = vm.ticketID > 0
                    ? '/CustomerService/CustomerSupportTickets/EditTicket'
                    : '/CustomerService/CustomerSupportTickets/CreateTicket';

                try {
                    const res = await fetch(url, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(vm)
                    });

                    if (!res.ok) {
                        alert('儲存失敗');
                        $('#saveTicket').prop('disabled', false);
                        return;
                    }

                    const result = await res.json();
                    ticketModal.hide();
                    ticketTable.ajax.reload();

                } catch (err) {
                    alert('儲存失敗');
                } finally {
                    $('#saveTicket').prop('disabled', false);
                }
            });

            // Modal 關閉時自動把焦點移回主頁新增工單按鈕（無障礙修正）
            ticketModalEl.addEventListener('hidden.bs.modal', function () {
                document.getElementById('btnAddTicket').focus();
            });

        });

    });
})();