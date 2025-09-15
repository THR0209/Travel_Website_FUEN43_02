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
    // 將評分轉成星號函數（表格列用）
    // ===============================
    function renderStarsTable(rating) {
        const r = Math.max(1, Math.min(5, rating || 1));
        const full = '★'.repeat(r);
        const empty = '☆'.repeat(5 - r);
        const displayRating = rating != null ? rating : 1;
        return `<span class="star-rating" data-rating="${displayRating}">${full}${empty}</span>`;
    }

    // ===============================
    // 將評分轉成星號函數（Modal 用）
    // ===============================
    function renderStarsModal(rating) {
        const r = Math.max(1, Math.min(5, rating || 1));
        const full = '★'.repeat(r);
        const empty = '☆'.repeat(5 - r);
        const displayRating = rating != null ? rating : 1;
        return `<span class="star-rating" data-rating="${displayRating}">${full}${empty}</span>`;
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
            { data: 'feedbackID', title: 'ID' },
            { data: 'ticketID', title: '工單編號' },
            {
                data: 'feedbackRating',
                title: '評分',
                render: function (data) {
                    return renderStarsTable(data);
                }
            },
            { data: 'feedbackComment', title: '留言' },
            {
                data: 'createTime',
                title: '建立時間',
                render: {
                    display: function (data) { return formatDateTime(data); },
                    sort: function (data) { return data ? new Date(data).getTime() : 0; },
                    filter: function (data) { return data || ''; }
                }
            },
            {
                data: 'feedbackID',
                className: 'text-end',
                orderable: false,
                render: function (id) {
                    return `
                        <button class="btn btn-secondary btn-sm btn-view" data-id="${id}">檢視</button>
                        <button class="btn btn-danger btn-sm btn-del" data-id="${id}">刪除</button>`;
                }
            }
        ],
        language: {
            lengthMenu: "顯示 _MENU_ 筆資料",
            zeroRecords: "無資料",
            info: "顯示 _START_ 到 _END_ 筆資料，共 _TOTAL_ 筆",
            infoEmpty: "顯示 0 到 0 筆資料，共 0 筆",
            infoFiltered: "(從 _MAX_ 筆資料中篩選)",
            search: "搜尋:", paginate: { first: "第一頁", last: "最後一頁", next: "下一頁", previous: "上一頁" },
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
            $('#detailRating').html(renderStarsModal(data.feedbackRating));
            $('#detailComment').text(data.feedbackComment || '(無留言)');
            $('#detailTime').text(formatDateTime(data.createTime));
            $('#btnDeleteFeedback').data('id', data.feedbackID);
            $('#feedbackModal').modal('show');
        }).fail(function () {
            alert('取得評價資料失敗');
        });
    });

    // ===============================
    // 刪除按鈕事件
    // ===============================
    $('#feedbackTable').on('click', '.btn-del', function () {
        const id = $(this).data('id');
        if (!confirm('確定要刪除這筆評價嗎？')) return;
        $.ajax({
            url: `/CustomerService/CustomerSupportFeedback/Delete/${id}`,
            type: 'DELETE',
            success: function () { table.ajax.reload(); },
            error: function () { alert('刪除失敗'); }
        });
    });

    $('#btnDeleteFeedback').on('click', function () {
        const id = $(this).data('id');
        if (!id) return;
        if (confirm('確定要刪除這筆評價嗎？')) {
            $.ajax({
                url: `/CustomerService/CustomerSupportFeedback/Delete/${id}`,
                type: 'DELETE',
                success: function () {
                    $('#feedbackModal').modal('hide');
                    table.ajax.reload();
                },
                error: function () { alert('刪除失敗'); }
            });
        }
    });

    // ===============================
    // 顯示/隱藏按鈕
    // ===============================
   
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) { // 滾動 200px 就顯示
            $('#backToTop').fadeIn();
        } else {
            $('#backToTop').fadeOut();
        }
    });

    // ===============================
    // 點擊回到頂部
    // ===============================

    $('#backToTop').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 200); // 500ms 平滑滾動
    });
})();





    //$(document).ready(function () {
    //    const table = $('#feedbackTable').DataTable({
    //        // 先關掉 ajax，直接塞假資料
    //        data: [
    //            { feedbackID: 1, ticketID: 1001, feedbackRating: 5, feedbackComment: "服務很好！", createTime: "2025-09-01 10:30" },
    //            { feedbackID: 2, ticketID: 1002, feedbackRating: 3, feedbackComment: "普通，速度還可以。", createTime: "2025-09-02 14:15" },
    //            { feedbackID: 3, ticketID: 1003, feedbackRating: 4, feedbackComment: "回覆詳盡。", createTime: "2025-09-03 09:00" },
    //            { feedbackID: 4, ticketID: 1004, feedbackRating: 2, feedbackComment: "等待太久。", createTime: "2025-09-04 17:45" },
    //            { feedbackID: 5, ticketID: 1005, feedbackRating: 1, feedbackComment: "不滿意，沒解決問題。", createTime: "2025-09-05 12:20" }
    //        ],
    //        columns: [
    //            { data: 'feedbackID' },
    //            { data: 'ticketID' },
    //            { data: 'feedbackRating' },
    //            { data: 'feedbackComment' },
    //            { data: 'createTime' },
    //            {
    //                data: 'feedbackID',
    //                className: 'text-end',
    //                render: function (id) {
    //                    return `
    //                        <button class="btn btn-secondary btn-sm btn-view" data-id="${id}">檢視</button>
    //                        <button class="btn btn-danger btn-sm btn-del" data-id="${id}">刪除</button>`;
    //                }
    //            }
    //        ],
    //        language: {
    //            lengthMenu: "顯示 _MENU_ 筆資料",
    //            zeroRecords: "無資料",
    //            info: "顯示 _START_ 到 _END_ 筆資料，共 _TOTAL_ 筆",
    //            infoEmpty: "顯示 0 到 0 筆資料，共 0 筆",
    //            infoFiltered: "(從 _MAX_ 筆資料中篩選)",
    //            search: "搜尋:", paginate: { first: "第一頁", last: "最後一頁", next: "下一頁", previous: "上一頁" },
    //            loadingRecords: "載入中...",
    //            processing: "處理中..."
    //        }
    //    });

    //    // 檢視 (假資料版本也可以用)
    //    $('#feedbackTable').on('click', '.btn-view', function () {
    //        const id = $(this).data('id');
    //        const row = table.data().toArray().find(x => x.feedbackID === id);

    //        if (row) {
    //            $('#detailTicket').text(row.ticketID);
    //            $('#detailRating').text(row.feedbackRating);
    //            $('#detailComment').text(row.feedbackComment || '(無留言)');
    //            $('#detailTime').text(row.createTime);
    //            $('#btnDeleteFeedback').data('id', row.feedbackID);
    //            $('#feedbackModal').modal('show');
    //        }
    //    });

    //    // 假刪除（不會真的打 API，只是前端移除資料）
    //    $('#feedbackTable').on('click', '.btn-del', function () {
    //        const id = $(this).data('id');
    //        if (confirm('確定要刪除這筆評價嗎？')) {
    //            table.rows((idx, data) => data.feedbackID === id).remove().draw();
    //        }
    //    });
    //});
