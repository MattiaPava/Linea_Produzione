namespace Linea_Produzione.Models
{
    public class DistintaBaseModel
    {
        public long Id { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataInizio { get; set; }
        public string Versione { get; set; }
        public string Stato { get; set; }

        // Variabile di controllo per abilitare/disabilitare i campi
        public bool ReadOnly { get; set; }
    }
}