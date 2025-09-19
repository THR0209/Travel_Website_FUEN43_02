$(function () {
    // ==== Bootstrap Modal 實例化 ====
    var faqModal = new bootstrap.Modal(document.getElementById('faqModal'), { focus: false });
    var categoryModal = new bootstrap.Modal(document.getElementById('categoryModal'), { focus: false });

    // ==== 狀態資料 ====
    var state = { categories: [], faqs: [] };
    var editingFaqId = null, editingCategoryId = null;

    // ==== API 方法 ====
    var api = {
        getCategories: () => $.getJSON("/CustomerService/FAQs/api/categories"),
        getFAQs: () => $.getJSON("/CustomerService/FAQs/api"),
        createFAQ: vm => $.ajax({ url: "/CustomerService/FAQs/api", type: "POST", contentType: "application/json", data: JSON.stringify(vm) }),
        updateFAQ: (id, vm) => $.ajax({ url: `/CustomerService/FAQs/api/${id}`, type: "PUT", contentType: "application/json", data: JSON.stringify(vm) }),
        deleteFAQ: id => $.ajax({ url: `/CustomerService/FAQs/api/${id}`, type: "DELETE" }),
        createCategory: vm => $.ajax({ url: "/CustomerService/FAQs/api/categories", type: "POST", contentType: "application/json", data: JSON.stringify(vm) }),
        updateCategory: (id, vm) => $.ajax({ url: `/CustomerService/FAQs/api/categories/${id}`, type: "PUT", contentType: "application/json", data: JSON.stringify(vm) }),
        deleteCategory: id => $.ajax({ url: `/CustomerService/FAQs/api/categories/${id}`, type: "DELETE" })
    };

    // ==== XSS 防護 ====
    function escapeHtml(s) {
        if (!s) return '';
        return String(s).replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));
    }

    // ==== 渲染分類選項 ====
    function renderCategoryOptions() {
        var catOpts = '<option value="">全部分類</option>';
        var formOpts = '<option value="">請選擇</option>';
        $.each(state.categories, (_, c) => {
            catOpts += `<option value="${c.id}">${escapeHtml(c.name)}</option>`;
            formOpts += `<option value="${c.id}">${escapeHtml(c.name)}</option>`;
        });
        $('#faqCategoryFilter').html(catOpts);
        $('#faqFormCategory').html(formOpts);
        $('#faqFormStatus').html(`
            <option value="">請選擇</option>
            <option value="上架">上架</option>
            <option value="下架">下架</option>
        `);
    }

    // ==== 初始化 FAQ DataTable ====
    function initFaqDataTable() {
        if ($.fn.DataTable.isDataTable('#faqTable')) $('#faqTable').DataTable().destroy();
        $('#faqTable').DataTable({
            paging: true,
            searching: false,
            ordering: true,
            info: true,
            autoWidth: false,
            dom: 'tip',
            language: {
                lengthMenu: "顯示 _MENU_ 筆資料",
                zeroRecords: "無資料",
                info: "顯示 _START_ 到 _END_ 筆資料，共 _TOTAL_ 筆",
                infoEmpty: "顯示 0 到 0 筆資料，共 0 筆",
                infoFiltered: "(從 _MAX_ 筆資料中篩選)",
                search: "搜尋:",
                paginate: { first: "第一頁", last: "最後一頁", next: "下一頁", previous: "上一頁" },
                loadingRecords: "載入中...", processing: "處理中..."
            }
        });
    }

    // ==== 渲染 FAQ 表格 ====
    function renderFaqTable(highlightId) {
        if (!$.fn.DataTable.isDataTable('#faqTable')) initFaqDataTable();
        var table = $('#faqTable').DataTable();
        table.clear();
        var kw = $('#faqSearch').val().trim().toLowerCase();
        var cat = $('#faqCategoryFilter').val();
        $.each(state.faqs, (_, f) => {
            var hay = ((f.question || "") + " " + (f.answer || "")).toLowerCase();
            var catMatch = !cat || String(f.categoryID) === cat;
            if (catMatch && (!kw || hay.includes(kw))) {
                var catName = (state.categories.find(c => String(c.id) === String(f.categoryID)) || {}).name || "-";
                var statusHtml = f.isActive ? '<span class="status done">上架</span>' : '<span class="status pending">下架</span>';
                var ops = `
                    <div class="btn-group-right">
                        <button class="btn btn-primary" data-act="edit" data-id="${f.faqID}">編輯</button>
                        <button class="btn btn-danger" data-act="del" data-id="${f.faqID}">刪除</button>
                    </div>`;
                var row = table.row.add([catName, escapeHtml(f.question), escapeHtml(f.answer), statusHtml, ops]).node();
                $(row).attr('data-id', f.faqID);
            }
        });
        table.draw();

        // 高亮新行
        if (highlightId) {
            var rowNode = $(table.rows().nodes()).filter(`[data-id='${highlightId}']`);
            rowNode.addClass('highlight');
            $('html, body').animate({ scrollTop: rowNode.offset().top - 100 }, 300);
        }
    }

    // ==== 初始化 Category DataTable ====
    // 這個 function 會重設（destroy）舊的 DataTable instance，如果已經存在，然後重新建立新的 DataTable。
    // paging: 分頁功能
    // searching: 關閉搜尋功能
    // ordering: 關閉欄位排序
    // info: 顯示資料筆數等資訊
    // autoWidth: 關閉自動寬度
    // dom: 設定 DataTables 控制項顯示模式（這裡 tip 指只顯示 table, info, pagination）
    function initCategoryDataTable() {
        if ($.fn.DataTable.isDataTable('#categoryTable')) $('#categoryTable').DataTable().destroy();
        $('#categoryTable').DataTable({
            paging: true,
            searching: false,
            ordering: false,
            info: true,
            autoWidth: false,
            dom: 'tip',
            language: {
                lengthMenu: "顯示 _MENU_ 筆資料",
                zeroRecords: "無資料",
                info: "顯示 _START_ 到 _END_ 筆資料，共 _TOTAL_ 筆",
                infoEmpty: "顯示 0 到 0 筆資料，共 0 筆",
                infoFiltered: "(從 _MAX_ 筆資料中篩選)",
                search: "搜尋:",
                paginate: { first: "第一頁", last: "最後一頁", next: "下一頁", previous: "上一頁" },
                loadingRecords: "載入中...", processing: "處理中..."
            }
        });
    }

    // ==== 渲染分類表格 ====
    // 這個 function 用 DataTables API 來加入每一筆分類資料
    // 不再用 html 拼 tr，只用 DataTables 的 row.add() 加入資料
    function renderCategoryTable(highlightId) {
        // 如果還沒初始化 DataTable，先初始化
        if (!$.fn.DataTable.isDataTable('#categoryTable')) initCategoryDataTable();
        var table = $('#categoryTable').DataTable();
        table.clear(); // 清空舊資料

        // 逐筆加入分類資料
        $.each(state.categories, (i, c) => {
            var ops = `
            <div class="btn-group-right">
                <button class="btn btn-primary" data-act="rename" data-id="${c.id}">編輯</button>
                <button class="btn btn-danger" data-act="c-del" data-id="${c.id}">刪除</button>
            </div>`;
            // row.add([#序號, 分類名稱, 操作欄])
            var row = table.row.add([i + 1, escapeHtml(c.name), ops]).node();
            // 設定每一行的 data-id 方便後續操作
            $(row).attr('data-id', c.id);
        });

        // DataTables 會自動顯示空表時的預設訊息，不需額外處理
        table.draw();

        // 若有指定 highlightId，高亮顯示新行
        if (highlightId) {
            var rowNode = $(table.rows().nodes()).filter(`[data-id='${highlightId}']`);
            rowNode.addClass('highlight');
            $('html, body').animate({ scrollTop: rowNode.offset().top - 100 }, 300);
        }
    }

    // ==== 載入所有資料 ====
    function fetchAndRenderAll() {
        $.when(api.getCategories(), api.getFAQs())
            .done((cats, faqs) => {
                state.categories = $.map(cats[0], c => ({ id: String(c.id), name: c.name }));
                state.faqs = $.map(faqs[0], f => ({
                    faqID: f.faqid,
                    question: f.question,
                    answer: f.answer,
                    categoryID: String(f.categoryID),
                    isActive: f.isActive,
                    createTime: f.createTime,
                    updateTime: f.updateTime,
                    categoryName: f.categoryName
                }));
                renderCategoryOptions();
                renderFaqTable();
                renderCategoryTable();
            })
            .fail(() => alert('載入 FAQ/分類失敗'));
    }

    // ==== FAQ Modal 開啟 ====
    function openFaqModal(faq) {
        editingFaqId = faq ? faq.faqID : null;
        $('#faqID').val(faq ? faq.faqID : '');
        $('#faqFormCategory').val(faq ? faq.categoryID : '');
        $('#faqFormStatus').val(faq ? (faq.isActive ? '上架' : '下架') : '');
        $('#faqFormQuestion').val(faq ? faq.question : '');
        $('#faqFormAnswer').val(faq ? faq.answer : '');
        faqModal.show();
    }

    // ==== FAQ Modal 儲存 ====
    $('#faqModalSave').on('click', function () {
        var categoryID = $('#faqFormCategory').val();
        var status = $('#faqFormStatus').val();
        var question = $('#faqFormQuestion').val().trim();
        var answer = $('#faqFormAnswer').val().trim();
        if (!categoryID) return alert('請選擇分類');
        if (!status) return alert('請選擇狀態');
        if (!question) return alert('請輸入問題');
        if (!answer) return alert('請輸入答案');

        var $btn = $(this).prop('disabled', true);
        var vm = {
            FAQID: $('#faqID').val() ? parseInt($('#faqID').val()) : 0,
            CategoryID: Number(categoryID),
            IsActive: status === '上架',
            Question: question,
            Answer: answer
        };
        var ajax = vm.FAQID > 0 ? api.updateFAQ(vm.FAQID, vm) : api.createFAQ(vm);
        ajax.done(() => { faqModal.hide(); fetchAndRenderAll(); })
            .fail(() => alert('儲存失敗'))
            .always(() => { $btn.prop('disabled', false); });
    });

    // ==== FAQ 事件綁定 ====
    $('#btnFaqNew').on('click', () => openFaqModal(null));
    $('#faqSearch').on('input', () => renderFaqTable());
    $('#faqCategoryFilter').on('change', () => renderFaqTable());
    $('#btnFaqClear').on('click', () => { $('#faqSearch').val(''); $('#faqCategoryFilter').val(''); renderFaqTable(); });

    $('#faqTable tbody').on('click', 'button', function () {
        var act = $(this).data('act'), id = $(this).data('id');
        if (!act || !id) return;
        if (act === 'edit') openFaqModal(state.faqs.find(f => String(f.faqID) === String(id)));
        else if (act === 'del' && confirm('確定要刪除？')) api.deleteFAQ(id).done(fetchAndRenderAll).fail(() => alert('刪除失敗'));
    });

    // ==== 分類 Modal 開啟/儲存/刪除 ====
    function openCategoryModal(cat) {
        editingCategoryId = cat ? cat.id : null;
        $('#categoryID').val(cat ? cat.id : "");
        $('#categoryFormName').val(cat ? cat.name : '');
        categoryModal.show();
    }

    $('#btnAddCategory').on('click', () => openCategoryModal(null));
    $('#categoryModalSave').on('click', function () {
        var name = $('#categoryFormName').val().trim();
        if (!name) return alert('請輸入分類名稱');
        var $btn = $(this).prop('disabled', true);
        var vm = { categoryID: $('#categoryID').val() || undefined, categoryName: name };
        var ajax = editingCategoryId ? api.updateCategory(editingCategoryId, vm) : api.createCategory(vm);
        ajax.done(() => { categoryModal.hide(); fetchAndRenderAll(); })
            .fail(() => alert('儲存分類失敗'))
            .always(() => $btn.prop('disabled', false));
    });

    $('#categoryTable tbody').on('click', 'button', function () {
        var act = $(this).data('act'), id = $(this).data('id');
        if (!act || !id) return;
        var cat = state.categories.find(c => String(c.id) === String(id));
        if (act === 'rename') openCategoryModal(cat);
        else if (act === 'c-del' && confirm('確定要刪除此分類？')) api.deleteCategory(id).done(fetchAndRenderAll).fail(() => alert('刪除分類失敗'));
    });

    // ==== 初次載入 ====
    fetchAndRenderAll();
});
