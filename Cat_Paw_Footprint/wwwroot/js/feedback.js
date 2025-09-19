(() => {
    // ===============================
    // 日期格式化函數
    // ===============================
    function formatDateTime(dateString) {
        if (!dateString) return '';
        const d = new Date(dateString);
        const yyyy = d.getFullYear();
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const dd = String(d.getDate()).padStart(2, '0');
        const hh = String(d.getHours()).padStart(2, '0');
        const min = String(d.getMinutes()).padStart(2, '0');
        return `${yyyy}/${mm}/${dd} ${hh}:${min}`;
    }

    // ===============================
    // 評分轉星號
    // ===============================
    function renderStars(rating) {
        const r = Math.max(1, Math.min(5, rating || 1));
        return `<span class="star-rating" data-rating="${rating || 1}">${'★'.repeat(r)}${'☆'.repeat(5 - r)}</span>`;
    }

    // ===============================
    // 初始化 DataTable
    // ===============================
    const table = $('#feedbackTable').DataTable({
        ajax: {
            url: '/CustomerService/CustomerSupportFeedback/GetAll',
            dataSrc: ''
        },
        columns: [
            { data: 'ticketID', title: '工單編號' },
            { data: 'feedbackRating', title: '評分', render: renderStars },
            { data: 'feedbackComment', title: '留言' },
            {
                data: 'createTime',
                title: '建立時間',
                render: function (data, type) {
                    if (type === 'display') return formatDateTime(data);
                    return data ? new Date(data).getTime() : 0;
                }
            },
            {
                data: 'feedbackID',
                
                orderable: false,
                render: function (id) {
                    return `
                        <button class="btn btn-primary btn-view" data-id="${id}">檢視</button>
                        <button class="btn btn-danger btn-del" data-id="${id}">刪除</button>`;
                }
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

    // ===============================
    // 檢視按鈕事件
    // ===============================
    $('#feedbackTable').on('click', '.btn-view', function () {
        const id = $(this).data('id');
        $.get(`/CustomerService/CustomerSupportFeedback/Details/${id}`, function (data) {
            $('#detailTicket').text(data.ticketID || '(無號碼)');
            $('#detailRating').html(renderStars(data.feedbackRating));
            $('#detailComment').text(data.feedbackComment || '(無留言)');
            $('#detailTime').text(formatDateTime(data.createTime));
            $('#btnDeleteFeedback').data('id', data.feedbackID);
            $('#feedbackModal').modal('show');
        }).fail(() => alert('取得評價資料失敗'));
    });

    // ===============================
    // 刪除按鈕事件
    // ===============================
    function deleteFeedback(id) {
        $.ajax({
            url: `/CustomerService/CustomerSupportFeedback/Delete/${id}`,
            type: 'DELETE',
            success: () => table.ajax.reload(),
            error: () => alert('刪除失敗')
        });
    }

    $('#feedbackTable').on('click', '.btn-del', function () {
        const id = $(this).data('id');
        if (confirm('確定要刪除這筆評價嗎？')) deleteFeedback(id);
    });

    $('#btnDeleteFeedback').on('click', function () {
        const id = $(this).data('id');
        if (id && confirm('確定要刪除這筆評價嗎？')) {
            deleteFeedback(id);
            $('#feedbackModal').modal('hide');
        }
    });
})();
