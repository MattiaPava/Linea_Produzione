namespace Linea_Produzione.Models
{
    public class DistintaBaseEntity
    {
        public long Id { get; set; }
        public long IdUtente { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataInizio { get; set; }
        public string Versione { get; set; }
        public short Stato { get; set; }
    }
}