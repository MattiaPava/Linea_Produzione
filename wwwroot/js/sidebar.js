// wwwroot/js/sidebar.js

function toggleSidebar() {
    var sidebar = document.getElementById("mySidebar");
    var main = document.getElementById("myMain");
    var fields = document.getElementById("sidebarFields");
    var btnCerca = document.getElementById("btnCerca");
    var btnCancella = document.getElementById("btnCancella");
    var icon = document.getElementById("toggleIcon");

    // Controllo di sicurezza: se non c'è la sidebar, esci
    if (!sidebar || !main) return;

    if (sidebar.classList.contains("col-md-3")) {
        // Riduci le colonne
        sidebar.classList.replace("col-md-3", "col-md-1");
        main.classList.replace("col-md-9", "col-md-11");

        // Nascondi gli elementi (solo se esistono nella pagina attuale)
        if (fields) fields.style.display = "none";
        if (btnCerca) btnCerca.style.display = "none";
        if (btnCancella) btnCancella.style.display = "none";
        if (icon) icon.className = "bi bi-chevron-right";
    } else {
        // Allarga le colonne
        sidebar.classList.replace("col-md-1", "col-md-3");
        main.classList.replace("col-md-11", "col-md-9");

        // Mostra gli elementi (solo se esistono nella pagina attuale)
        if (fields) fields.style.display = "block";
        if (btnCerca) btnCerca.style.display = "block";
        if (btnCancella) btnCancella.style.display = "block";
        if (icon) icon.className = "bi bi-chevron-left";
    }
}