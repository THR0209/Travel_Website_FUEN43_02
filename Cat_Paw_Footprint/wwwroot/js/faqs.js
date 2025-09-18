// FAQ 管理 JS（純 jQuery + Bootstrap Modal 控制）
// -----------------------------------------------

$(function () {
    // ==== Bootstrap Modal 實例化 ====
    var faqModal = new bootstrap.Modal(document.getElementById('faqModal'), { focus: false });
    var categoryModal = new bootstrap.Modal(document.getElementById('categoryModal'), { focus: false });

    // ==== 狀態資料 ====
    var state = { categories: [], faqs: [] };
    var editingFaqId = null;
    var editingCategoryId = null;

    // ==== API 方法 ==== 
    var api = {
        getCategories: function () { return $.getJSON("/CustomerService/FAQs/api/categories"); },
        getFAQs: function () { return $.getJSON("/CustomerService/FAQs/api"); },
        createFAQ: function (vm) { return $.ajax({ url: "/CustomerService/FAQs/api", type: "POST", contentType: "application/json", data: JSON.stringify(vm) }); },
        updateFAQ: function (id, vm) { return $.ajax({ url: `/CustomerService/FAQs/api/${id}`, type: "PUT", contentType: "application/json", data: JSON.stringify(vm) }); },
        deleteFAQ: function (id) { return $.ajax({ url: `/CustomerService/FAQs/api/${id}`, type: "DELETE" }); },
        createCategory: function (vm) { return $.ajax({ url: "/CustomerService/FAQs/api/categories", type: "POST", contentType: "application/json", data: JSON.stringify(vm) }); },
        updateCategory: function (id, vm) { return $.ajax({ url: `/CustomerService/FAQs/api/categories/${id}`, type: "PUT", contentType: "application/json", data: JSON.stringify(vm) }); },
        deleteCategory: function (id) { return $.ajax({ url: `/CustomerService/FAQs/api/categories/${id}`, type: "DELETE" }); }
    };

    // ==== XSS 防護 ====
    function escapeHtml(s) {
        if (!s) return '';
        return String(s).replace(/[&<>"']/g, function (m) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m];
        });
    }

    // ==== 下拉分類選項渲染 ====
    function renderCategoryOptions() {
        var catOpts = '<option value="">全部分類</option>';
        var formOpts = '<option value="">請選擇</option>';
        $.each(state.categories, function (_, c) {
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

    // ==== FAQ DataTable 初始化 ====
    function initFaqDataTable() {
        if ($.fn.DataTable.isDataTable('#faqTable')) $('#faqTable').DataTable().destroy();
        $('#faqTable').DataTable({
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
                loadingRecords: "載入中...",
                processing: "處理中..."
            }
        });
    }

    // ==== FAQ 表格渲染 ====
    function renderFaqTable(highlightId) {
        if (!$.fn.DataTable.isDataTable('#faqTable')) initFaqDataTable();
        var table = $('#faqTable').DataTable();
        table.clear();

        var kw = $('#faqSearch').val().trim().toLowerCase();
        var cat = $('#faqCategoryFilter').val();

        $.each(state.faqs, function (_, f) {
            var hay = ((f.question || "") + " " + (f.answer || "")).toLowerCase();
            var catMatch = !cat || String(f.categoryID) === cat;
            if (catMatch && (!kw || hay.includes(kw))) {
                var catName = (state.categories.find(function (c) { return String(c.id) === String(f.categoryID); }) || {}).name || "-";
                var statusHtml = f.isActive ? '<span class="status done">上架</span>' : '<span class="status pending">下架</span>';
                var ops = `<div class="btn-group-right">
                    <button class="btn btn-primary" data-act="edit" data-id="${f.faqID}">編輯</button>
                    <button class="btn btn-danger" data-act="del" data-id="${f.faqID}">刪除</button>
                </div>`;
                var row = table.row.add([
                    catName,
                    escapeHtml(f.question),
                    escapeHtml(f.answer),
                    statusHtml,
                    ops
                ]).node();
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

    // ==== 分類表格渲染 ====
    function renderCategoryTable(highlightId) {
        var html = '';
        $.each(state.categories, function (i, c) {
            html += `<tr data-id="${c.id}">
                <td>${i + 1}</td>
                <td>${escapeHtml(c.name)}</td>
                <td>
                    <div class="btn-group-right">
                        <button class="btn btn-primary" data-act="rename" data-id="${c.id}">編輯</button>
                        <button class="btn btn-danger" data-act="c-del" data-id="${c.id}">刪除</button>
                    </div>
                </td>
            </tr>`;
        });
        if (!html) html = '<tr><td colspan="3" style="text-align:center;color:#888">無分類</td></tr>';
        $('#categoryTable tbody').html(html);

        // 高亮新行
        if (highlightId) {
            var row = $(`#categoryTable tbody tr[data-id='${highlightId}']`);
            if (row.length) {
                row.addClass('highlight');
                $('html, body').animate({ scrollTop: row.offset().top - 100 }, 300);
            }
        }
    }

    // ==== FAQ/分類 資料載入 ====
    function fetchAndRenderAll() {
        $.when(api.getCategories(), api.getFAQs()).done(function (cats, faqs) {
            state.categories = $.map(cats[0], function (c) { return { id: String(c.id), name: c.name }; });
            state.faqs = $.map(faqs[0], function (f) {
                return {
                    faqID: f.faqid,
                    question: f.question,
                    answer: f.answer,
                    categoryID: String(f.categoryID),
                    isActive: f.isActive,
                    createTime: f.createTime,
                    updateTime: f.updateTime,
                    categoryName: f.categoryName
                };
            });
            renderCategoryOptions();
            renderFaqTable();
            renderCategoryTable();
        }).fail(function () {
            alert('載入 FAQ/分類失敗');
        });
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

        ajax.done(function (result) {
            faqModal.hide();
            fetchAndRenderAll();
        }).fail(function () {
            alert('儲存失敗');
        }).always(function () {
            $btn.prop('disabled', false);
        });
    });

    // ==== FAQ Modal 開啟/搜尋/清除 ====
    $('#btnFaqNew').on('click', function () { openFaqModal(null); });
    $('#faqSearch').on('input', function () { renderFaqTable(); });
    $('#faqCategoryFilter').on('change', function () { renderFaqTable(); });
    $('#btnFaqClear').on('click', function () {
        $('#faqSearch').val('');
        $('#faqCategoryFilter').val('');
        renderFaqTable();
    });

    // ==== FAQ 編輯/刪除 ====
    $('#faqTable tbody').on('click', 'button', function () {
        var act = $(this).data('act'), id = $(this).data('id');
        if (!act || !id) return;
        if (act === 'edit') {
            var faq = state.faqs.find(function (f) { return String(f.faqID) === String(id); });
            openFaqModal(faq);
        }
        else if (act === 'del') {
            if (!confirm('確定要刪除？')) return;
            api.deleteFAQ(id).done(function () {
                fetchAndRenderAll();
            }).fail(function () {
                alert('刪除失敗');
            });
        }
    });

    // ==== 分類 Modal 開啟 ====
    function openCategoryModal(cat) {
        editingCategoryId = cat ? cat.id : null;
        $('#categoryID').val(cat ? cat.id : "");
        $('#categoryFormName').val(cat ? cat.name : '');
        categoryModal.show();
    }
    $('#btnAddCategory').on('click', function () { openCategoryModal(null); });

    // ==== 分類 Modal 儲存 ====
    $('#categoryModalSave').on('click', function () {
        var name = $('#categoryFormName').val().trim();
        if (!name) return alert('請輸入分類名稱');
        var $btn = $(this).prop('disabled', true);

        var vm = {
            categoryID: $('#categoryID').val() ? $('#categoryID').val() : undefined,
            categoryName: name
        };
        var ajax, url, method;
        if (editingCategoryId) {
            ajax = api.updateCategory(editingCategoryId, vm);
        } else {
            ajax = api.createCategory(vm);
        }
        ajax.done(function (result) {
            categoryModal.hide();
            fetchAndRenderAll();
        }).fail(function () {
            alert('儲存分類失敗');
        }).always(function () {
            $btn.prop('disabled', false);
        });
    });

    // ==== 分類 編輯/刪除 ====
    $('#categoryTable tbody').on('click', 'button', function () {
        var act = $(this).data('act'), id = $(this).data('id');
        if (!act || !id) return;
        var cat = state.categories.find(function (c) { return String(c.id) === String(id); });
        if (act === 'rename') openCategoryModal(cat);
        else if (act === 'c-del') {
            if (!confirm('確定要刪除此分類？')) return;
            api.deleteCategory(id).done(function () {
                fetchAndRenderAll();
            }).fail(function () {
                alert('刪除分類失敗');
            });
        }
    });

    // ==== 初次載入 ====
    fetchAndRenderAll();
});