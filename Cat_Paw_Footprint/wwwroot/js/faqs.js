(() => {
    // ==== DOM 快速選取 (不用覆蓋 $，保留 jQuery) ====
    const qs = (q, r = document) => r.querySelector(q);
    const qsa = (q, r = document) => Array.from(r.querySelectorAll(q));

    // ==== 所有用到的元素 ====
    const els = {
        faqTableBody: qs("#faqTable tbody"),
        faqSearch: qs("#faqSearch"),
        faqClear: qs("#btnFaqClear"),
        faqCategoryFilter: qs("#faqCategoryFilter"),
        btnFaqNew: qs("#btnFaqNew"),
        faqModal: qs("#faqModal"),
        faqModalTitle: qs("#faqModalTitle"),
        faqModalCancel: qs("#faqModalCancel"),
        faqModalSave: qs("#faqModalSave"),
        faqFormCategory: qs("#faqFormCategory"),
        faqFormStatus: qs("#faqFormStatus"),
        faqFormQuestion: qs("#faqFormQuestion"),
        faqFormAnswer: qs("#faqFormAnswer"),
        categoryTableBody: qs("#categoryTable tbody"),
        btnAddCategory: qs("#btnAddCategory"),
        categoryModal: qs("#categoryModal"),
        categoryModalCancel: qs("#categoryModalCancel"),
        categoryModalSave: qs("#categoryModalSave"),
        categoryFormName: qs("#categoryFormName"),
    };

    // ==== 狀態 ====
    let state = { categories: [], faqs: [] };
    let editingFaqId = null;
    let editingCategoryId = null;

    // ==== API 方法 ====
    const api = {
        getCategories: async () => (await fetch("/CustomerService/FAQs/api/categories")).json(),
        getFAQs: async () => (await fetch("/CustomerService/FAQs/api")).json(),
        createFAQ: async (vm) => fetch("/CustomerService/FAQs/api", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(vm) }).then(r => r.json()),
        updateFAQ: async (id, vm) => fetch(`/CustomerService/FAQs/api/${id}`, { method: "PUT", headers: { "Content-Type": "application/json" }, body: JSON.stringify(vm) }).then(r => r.json()),
        deleteFAQ: async (id) => fetch(`/CustomerService/FAQs/api/${id}`, { method: "DELETE" }).then(r => r.json()),
        createCategory: async (vm) => fetch("/CustomerService/FAQs/api/categories", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(vm) }).then(r => r.json()),
        updateCategory: async (id, vm) => fetch(`/CustomerService/FAQs/api/categories/${id}`, { method: "PUT", headers: { "Content-Type": "application/json" }, body: JSON.stringify(vm) }).then(r => r.json()),
        deleteCategory: async (id) => fetch(`/CustomerService/FAQs/api/categories/${id}`, { method: "DELETE" }).then(r => r.json())
    };

    // ==== XSS 防護 ====
    function escapeHtml(s) { if (!s) return ''; return String(s).replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m])); }

    // ==== 下拉分類選項渲染 ====
    function renderCategoryOptions() {
        els.faqCategoryFilter.innerHTML = ['<option value="">全部分類</option>']
            .concat(state.categories.map(c => `<option value="${c.id}">${escapeHtml(c.name)}</option>`))
            .join('');

        els.faqFormCategory.innerHTML = ['<option value="">請選擇</option>']
            .concat(state.categories.map(c => `<option value="${c.id}">${escapeHtml(c.name)}</option>`))
            .join('');

        els.faqFormStatus.innerHTML = `
            <option value="">請選擇</option>
            <option value="上架">上架</option>
            <option value="下架">下架</option>
        `;
    }

    // ==== FAQ 表格渲染 ====
    function renderFaqTable() {
        const kw = els.faqSearch.value.trim().toLowerCase();
        const cat = els.faqCategoryFilter.value;
        const rows = state.faqs.filter(f => {
            const hay = ((f.question || "") + " " + (f.answer || "")).toLowerCase();
            const catMatch = !cat || String(f.categoryID) === cat;
            return catMatch && (!kw || hay.includes(kw));
        }).map(f => {
            const catName = (state.categories.find(c => String(c.id) === String(f.categoryID)) || {}).name || "-";
            const statusHtml = f.isActive ? '<span class="status done">上架</span>' : '<span class="status pending">下架</span>';
            return `<tr data-id="${f.faqID}">
                <td>${escapeHtml(catName)}</td>
                <td>${escapeHtml(f.question)}</td>
                <td>${escapeHtml(f.answer)}</td>
                <td>${statusHtml}</td>
                <td>
                    <button class="btn btn-secondary" data-act="edit" data-id="${f.faqID}">編輯</button>
                    <button class="btn btn-danger" data-act="del" data-id="${f.faqID}">刪除</button>
                </td>
            </tr>`;
        }).join('');

        els.faqTableBody.innerHTML = rows || '<tr><td colspan="5" style="text-align:center;color:#888">無資料</td></tr>';
        initFaqDataTable();
    }

    // ==== 分類表格渲染 ====
    function renderCategoryTable() {
        els.categoryTableBody.innerHTML = state.categories.map((c, i) => `<tr data-id="${c.id}">
            <td>${i + 1}</td>
            <td>${escapeHtml(c.name)}</td>
            <td>
                <button class="btn btn-secondary" data-act="rename" data-id="${c.id}">編輯</button>
                <button class="btn btn-danger" data-act="c-del" data-id="${c.id}">刪除</button>
            </td>
        </tr>`).join('') || '<tr><td colspan="3" style="text-align:center;color:#888">無分類</td></tr>';
    }

    // ==== 初始化 FAQ DataTable ====
    function initFaqDataTable() {
        if (typeof jQuery === 'undefined' || !$.fn.DataTable) return console.error('DataTable 尚未載入');
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

    // ==== Modal 管理 ====
    function showModal(modal) { modal.classList.add('show'); }
    function hideModal(modal) { modal.classList.remove('show'); }

    // ==== 資料初次載入 ====
    async function fetchAndRenderAll() {
        try {
            const [cats, faqs] = await Promise.all([api.getCategories(), api.getFAQs()]);
            state.categories = cats.map(c => ({ id: String(c.id), name: c.name }));
            state.faqs = faqs.map(f => ({
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
        } catch (err) {
            console.error('載入 FAQ/分類失敗', err);
            alert('載入資料失敗');
        }
    }

    // ==== FAQ 事件綁定 ====
    els.btnFaqNew.addEventListener('click', () => openFaqModal(null));
    els.faqSearch.addEventListener('input', renderFaqTable);
    els.faqCategoryFilter.addEventListener('change', renderFaqTable);
    els.faqClear.addEventListener('click', () => { els.faqSearch.value = ''; els.faqCategoryFilter.value = ''; renderFaqTable(); });

    function openFaqModal(faq) {
        editingFaqId = faq ? faq.faqID : null;
        els.faqModalTitle.textContent = editingFaqId ? '編輯 FAQ' : '新增 FAQ';
        els.faqFormCategory.value = faq ? faq.categoryID : '';
        els.faqFormStatus.value = faq ? (faq.isActive ? '上架' : '下架') : '';
        els.faqFormQuestion.value = faq ? faq.question : '';
        els.faqFormAnswer.value = faq ? faq.answer : '';
        showModal(els.faqModal);
    }
    function closeFaqModal() { hideModal(els.faqModal); editingFaqId = null; els.faqFormQuestion.value = ''; els.faqFormAnswer.value = ''; }
    els.faqModalCancel.addEventListener('click', closeFaqModal);

    els.faqModalSave.addEventListener('click', async () => {
        const categoryID = els.faqFormCategory.value;
        const isActive = els.faqFormStatus.value === '上架';
        const question = els.faqFormQuestion.value.trim();
        const answer = els.faqFormAnswer.value.trim();
        if (!question) return alert('請輸入問題');
        if (!answer) return alert('請輸入答案');
        if (!categoryID) return alert('請選擇分類');
        if (!els.faqFormStatus.value) return alert('請選擇狀態');

        const vm = { FAQID: editingFaqId ?? 0, CategoryID: Number(categoryID), IsActive: isActive, Question: question, Answer: answer };
        try {
            const result = editingFaqId ? await api.updateFAQ(editingFaqId, vm) : await api.createFAQ(vm);
            if (result && result.message) alert(result.message);
            closeFaqModal();
            await fetchAndRenderAll();
        } catch { alert('儲存失敗'); }
    });

    els.faqTableBody.addEventListener('click', async e => {
        const act = e.target.dataset.act, id = e.target.dataset.id;
        if (!act || !id) return;
        if (act === 'edit') { const faq = state.faqs.find(f => String(f.faqID) === String(id)); openFaqModal(faq); }
        else if (act === 'del') { if (!confirm('確定要刪除？')) return; try { const r = await api.deleteFAQ(id); if (r && r.message) alert(r.message); await fetchAndRenderAll(); } catch { alert('刪除失敗'); } }
    });

    // ==== 分類 Modal ====
    function openCategoryModal(cat) { editingCategoryId = cat ? cat.id : null; els.categoryFormName.value = cat ? cat.name : ''; showModal(els.categoryModal); }
    function closeCategoryModal() { hideModal(els.categoryModal); editingCategoryId = null; els.categoryFormName.value = ''; }
    els.categoryModalCancel.addEventListener('click', closeCategoryModal);

    els.btnAddCategory.addEventListener('click', () => openCategoryModal(null));
    els.categoryModalSave.addEventListener('click', async () => {
        const name = els.categoryFormName.value.trim();
        if (!name) return alert('請輸入分類名稱');
        try {
            const result = editingCategoryId ? await api.updateCategory(editingCategoryId, { categoryID: editingCategoryId, categoryName: name }) : await api.createCategory({ categoryName: name });
            if (result && result.message) alert(result.message);
            closeCategoryModal();
            await fetchAndRenderAll();
        } catch { alert('儲存分類失敗'); }
    });

    els.categoryTableBody.addEventListener('click', async e => {
        const act = e.target.dataset.act, id = e.target.dataset.id;
        if (!act || !id) return;
        const cat = state.categories.find(c => String(c.id) === String(id));
        if (act === 'rename') openCategoryModal(cat);
        else if (act === 'c-del') { if (!confirm('確定要刪除此分類？')) return; try { const r = await api.deleteCategory(id); if (r && r.message) alert(r.message); await fetchAndRenderAll(); } catch { alert('刪除分類失敗'); } }
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

    // ==== 初次載入 ====
    fetchAndRenderAll();

})();
