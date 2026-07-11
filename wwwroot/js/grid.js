// wwwroot/js/grid.js

const GridManager = {
    _cache: {},

    initSelection: function (tbodyId, btnModificaId, btnEliminaId, callbacks) {
        const tbody = document.getElementById(tbodyId);
        if (!tbody) return;

        this._cache[tbodyId] = {
            btnModificaId: btnModificaId,
            btnEliminaId: btnEliminaId,
            callbacks: callbacks
        };

        this.resetButtons(btnModificaId, btnEliminaId);

        tbody.addEventListener("click", (e) => {
            const tr = e.target.closest("tr");
            if (!tr || !tr.hasAttribute("data-id")) return;

            const btnModifica = document.getElementById(btnModificaId);
            const btnElimina = document.getElementById(btnEliminaId);
            const giaSelezionata = tr.classList.contains("table-info");
            const idSelezionato = tr.getAttribute("data-id");

            tbody.querySelectorAll("tr").forEach(r => r.classList.remove("table-info"));

            if (!giaSelezionata) {
                tr.classList.add("table-info");
                if (btnModifica) btnModifica.disabled = false;
                if (btnElimina) btnElimina.disabled = false;

                if (callbacks && typeof callbacks.onSelect === "function") {
                    callbacks.onSelect(idSelezionato);
                }
            } else {
                tr.classList.remove("table-info");
                if (btnModifica) btnModifica.disabled = true;
                if (btnElimina) btnElimina.disabled = true;

                if (callbacks && typeof callbacks.onDeselect === "function") {
                    callbacks.onDeselect();
                }
            }
        });
    },

    resetButtons: function (btnModificaId, btnEliminaId) {
        const btnModifica = document.getElementById(btnModificaId);
        const btnElimina = document.getElementById(btnEliminaId);
        if (btnModifica) btnModifica.disabled = true;
        if (btnElimina) btnElimina.disabled = true;
    },

    /**
     * Svuota forzatamente la selezione attiva e 
     * notifica la pagina tramite il callback onDeselect.
     */
    clearSelection: function (tbodyId) {
        const config = this._cache[tbodyId];
        if (!config) return;

        this.resetButtons(config.btnModificaId, config.btnEliminaId);

        if (config.callbacks && typeof config.callbacks.onDeselect === "function") {
            config.callbacks.onDeselect();
        }
    },

    checkAutoSelect: function (tbodyId) {
        const tbody = document.getElementById(tbodyId);
        if (!tbody) return;

        const config = this._cache[tbodyId];
        if (!config) return;

        const rows = tbody.querySelectorAll("tr[data-id]");

        if (rows.length === 1) {
            const tr = rows[0];
            if (tr.classList.contains("table-info")) return;

            tr.classList.add("table-info");

            const btnModifica = document.getElementById(config.btnModificaId);
            const btnElimina = document.getElementById(config.btnEliminaId);
            if (btnModifica) btnModifica.disabled = false;
            if (btnElimina) btnElimina.disabled = false;

            if (config.callbacks && typeof config.callbacks.onSelect === "function") {
                const idSelezionato = tr.getAttribute("data-id");
                config.callbacks.onSelect(idSelezionato);
            }
        }
    }
};