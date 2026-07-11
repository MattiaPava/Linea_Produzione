// wwwroot/js/search.js

const SearchManager = (function () {
    let _config = {};

    function init(config) {
        _config = config;
        const form = document.getElementById(_config.formId);
        if (!form) return;

        // Gestione Submit
        form.addEventListener("submit", function (e) {
            e.preventDefault();
            const hiddenPage = document.getElementById("hiddenPage");
            if (hiddenPage) hiddenPage.value = 1; // Resetta pagina a 1 su nuova ricerca
            eseguiChiamata();
        });

        // Gestione Cancella
        const btnCancella = document.getElementById(_config.btnCancellaId);
        if (btnCancella) {
            btnCancella.addEventListener("click", function () {
                form.reset();
                const hiddenPage = document.getElementById("hiddenPage");
                const hiddenPageSize = document.getElementById("hiddenPageSize");
                const selectPageSize = document.getElementById("selectPageSize");

                if (hiddenPage) hiddenPage.value = 1;
                if (hiddenPageSize && selectPageSize) {
                    hiddenPageSize.value = "20";
                    selectPageSize.value = "20";
                }
                eseguiChiamata();
            });
        }
    }

    function eseguiChiamata() {
        const form = document.getElementById(_config.formId);
        const tbody = document.getElementById(_config.tbodyId);
        if (!form || !tbody) return;

        // Reset pulsanti di riga
        if (typeof GridManager !== "undefined" && GridManager.deselezionaCorrente) {
            GridManager.deselezionaCorrente();
        }

        tbody.innerHTML = `
            <tr>
                <td colspan="${_config.colspan}" class="text-center py-4 text-muted">
                    <div class="spinner-border text-primary spinner-border-sm me-2"></div> Ricerca in corso...
                </td>
            </tr>`;

        const formData = new FormData(form);
        const searchParams = new URLSearchParams(formData);

        fetch(`${_config.url}?${searchParams.toString()}`, {
            method: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .then(res => {
                if (!res.ok) throw new Error(`Errore Server: ${res.status}`);
                return res.json();
            })
            .then(data => {
                tbody.innerHTML = "";

                // Legge i dati sia che arrivino nel nuovo formato paginato (data.items), sia nel vecchio (data)
                const items = Array.isArray(data) ? data : (data.items || []);

                if (items.length === 0) {
                    tbody.innerHTML = `<tr><td colspan="${_config.colspan}" class="text-center text-muted py-4">${_config.emptyMessage}</td></tr>`;
                    if (typeof window.aggiornaInterfacciaPaginazione === "function") {
                        window.aggiornaInterfacciaPaginazione({ currentPage: 1, totalPages: 1, totalItems: 0 });
                    }
                    return;
                }

                // Render delle righe
                let htmlRows = "";
                items.forEach(item => {
                    htmlRows += _config.renderRow(item);
                });
                tbody.innerHTML = htmlRows;

                // Aggiorna la barra della paginazione (se i metadati esistono)
                if (typeof window.aggiornaInterfacciaPaginazione === "function" && data.totalPages) {
                    window.aggiornaInterfacciaPaginazione({
                        currentPage: data.currentPage,
                        totalPages: data.totalPages,
                        totalItems: data.totalItems
                    });
                }
            })
            .catch(err => {
                console.error("Errore AJAX in SearchManager:", err);
                tbody.innerHTML = `
                <tr>
                    <td colspan="${_config.colspan}" class="text-center text-danger py-4">
                        <i class="bi bi-exclamation-triangle fs-3 d-block mb-2"></i>
                        Impossibile recuperare i dati dal server.<br>
                        <small class="text-muted">${err.message}</small>
                    </td>
                </tr>`;
            });
    }

    return { init: init, triggerSearch: eseguiChiamata };
})();