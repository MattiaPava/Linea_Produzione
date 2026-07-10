// wwwroot/js/search.js

const SearchManager = {
    /**
     * Inizializza la gestione della ricerca asincrona per la pagina.
     * @param {Object} config - Oggetto di configurazione della ricerca.
     */
    init: function (config) {
        const form = document.getElementById(config.formId || "searchForm");
        if (!form) return;

        // 1. Intercetta l'evento submit del form
        form.addEventListener("submit", (e) => {
            e.preventDefault();
            this.execute(config);
        });

        // 2. Intercetta il pulsante Cancella/Reset se presente
        const btnCancella = document.getElementById(config.btnCancellaId || "btnCancella");
        if (btnCancella) {
            btnCancella.addEventListener("click", () => {
                form.reset();
                this.execute(config);
            });
        }

        // 3. Esegue la ricerca iniziale al caricamento della pagina
        this.execute(config);
    },

    /**
     * Esegue la chiamata asincrona e aggiorna la griglia.
     */
    execute: function (config) {
        // Reset dei pulsanti azione tramite GridManager (se presente nel progetto)
        if (typeof GridManager !== "undefined") {
            GridManager.resetButtons(config.btnModificaId, config.btnEliminaId);
        }

        const form = document.getElementById(config.formId || "searchForm");
        const tbody = document.getElementById(config.tbodyId || "gridBody");
        if (!form || !tbody) return;

        const formData = new FormData(form);
        const queryParams = new URLSearchParams(formData).toString();
        const colspan = config.colspan || 7;

        // Render dello spinner di caricamento
        tbody.innerHTML = `
            <tr>
                <td colspan="${colspan}" class="text-center py-5">
                    <div class="spinner-border text-primary" role="status"></div>
                </td>
            </tr>`;

        // Chiamata FETCH verso l'endpoint configurato
        fetch(`${config.url}?${queryParams}`)
            .then(response => {
                if (!response.ok) throw new Error("Errore di rete");
                return response.json();
            })
            .then(data => {
                tbody.innerHTML = ""; // Svuota lo spinner

                // Caso: Nessun risultato trovato
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

                // Caso: Dati presenti. Cicla l'array e usa la funzione custom di rendering
                data.forEach(item => {
                    const rowHTML = config.renderRow(item);
                    tbody.insertAdjacentHTML('beforeend', rowHTML);
                });
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