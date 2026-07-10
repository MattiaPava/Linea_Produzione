// wwwroot/js/search.js

const SearchManager = {
    init: function (config) {
        const form = document.getElementById(config.formId || "searchForm");
        if (!form) return;

        form.addEventListener("submit", (e) => {
            e.preventDefault();
            this.execute(config);
        });

        const btnCancella = document.getElementById(config.btnCancellaId || "btnCancella");
        if (btnCancella) {
            btnCancella.addEventListener("click", () => {
                form.reset();
                this.execute(config);
            });
        }

        this.execute(config);
    },

    execute: function (config) {
        const tbodyId = config.tbodyId || "gridBody";

        // ==========================================
        // MODIFICA: Svuota la selezione prima del Fetch.
        // Questo spegne i pulsanti e NASCONDE i tab.
        // ==========================================
        if (typeof GridManager !== "undefined") {
            GridManager.clearSelection(tbodyId);
        }

        const form = document.getElementById(config.formId || "searchForm");
        const tbody = document.getElementById(tbodyId);
        if (!form || !tbody) return;

        const formData = new FormData(form);
        const queryParams = new URLSearchParams(formData).toString();
        const colspan = config.colspan || 7;

        tbody.innerHTML = `
            <tr>
                <td colspan="${colspan}" class="text-center py-5">
                    <div class="spinner-border text-primary" role="status"></div>
                </td>
            </tr>`;

        fetch(`${config.url}?${queryParams}`)
            .then(response => {
                if (!response.ok) throw new Error("Errore di rete");
                return response.json();
            })
            .then(data => {
                tbody.innerHTML = "";

                if (!data || data.length === 0) {
                    tbody.innerHTML = `
                        <tr>
                            <td colspan="${colspan}" class="text-center text-muted py-4">
                                <i class="bi bi-search display-6 d-block mb-2"></i>
                                ${config.emptyMessage || "Nessun record corrisponde ai criteri di ricerca."}
                            </td>
                        </tr>`;
                    return;
                }

                data.forEach(item => {
                    const rowHTML = config.renderRow(item);
                    tbody.insertAdjacentHTML('beforeend', rowHTML);
                });

                // Se l'esito della nuova ricerca produce 1 sola riga, la ri-seleziona e ri-apre i tab
                if (typeof GridManager !== "undefined") {
                    GridManager.checkAutoSelect(tbodyId);
                }
            })
            .catch(err => {
                console.error("SearchManager Error:", err);
                tbody.innerHTML = `
                    <tr>
                        <td colspan="${colspan}" class="text-center text-danger py-4">
                            <i class="bi bi-exclamation-triangle display-6 d-block mb-2"></i>
                            Impossibile recuperare i dati dal server.
                        </td>
                    </tr>`;
            });
    }
};