using System;

namespace Linea_Produzione.Models
{
    public class DistintaBaseGridModel
    {
        public long Id { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataInizio { get; set; }
        public string Versione { get; set; } // Aggiunto per la tabella
        public string Stato { get; set; }    // Aggiunto per la tabella
    }
}