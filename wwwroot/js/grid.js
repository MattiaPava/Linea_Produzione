// wwwroot/js/grid.js

const GridManager = {
    /**
     * Inizializza l'ascoltatore di eventi per la selezione delle righe della griglia.
     * Gestisce l'evidenziazione e l'abilitazione/disabilitazione condizionale dei pulsanti d'azione.
     */
    initSelection: function (tbodyId, btnModificaId, btnEliminaId) {
        const tbody = document.getElementById(tbodyId);
        if (!tbody) return; // Se la griglia non esiste nella pagina corrente, si ferma senza errori

        // Applica il reset iniziale dei pulsanti al caricamento
        this.resetButtons(btnModificaId, btnEliminaId);

        // Gestione click con Event Delegation
        tbody.addEventListener("click", (e) => {
            const tr = e.target.closest("tr");

            // Salta il click se non è una riga di dati valida (es. spinner o nessun risultato)
            if (!tr || !tr.hasAttribute("data-id")) return;

            const btnModifica = document.getElementById(btnModificaId);
            const btnElimina = document.getElementById(btnEliminaId);
            const giaSelezionata = tr.classList.contains("table-info");

            // Rimuove l'evidenziazione da tutte le righe della tabella corrente
            tbody.querySelectorAll("tr").forEach(r => r.classList.remove("table-info"));

            if (!giaSelezionata) {
                // Seleziona la riga (Azzurro) e abilita i pulsanti se esistono nella DOM
                tr.classList.add("table-info");
                if (btnModifica) btnModifica.disabled = false;
                if (btnElimina) btnElimina.disabled = false;
            } else {
                // Deseleziona e disabilita i pulsanti
                if (btnModifica) btnModifica.disabled = true;
                if (btnElimina) btnElimina.disabled = true;
            }
        });
    },

    /**
     * Disabilita in sicurezza i pulsanti passati se sono presenti nella pagina.
     */
    resetButtons: function (btnModificaId, btnEliminaId) {
        const btnModifica = document.getElementById(btnModificaId);
        const btnElimina = document.getElementById(btnEliminaId);

        if (btnModifica) btnModifica.disabled = true;
        if (btnElimina) btnElimina.disabled = true;
    }
};