using System;

namespace Linea_Produzione.Models
{
    public class DistintaBaseSearchModel
    {
        public long? Id { get; set; }
        public List<long> Ids { get; set; }
        public long? IdUtente { get; set; }
        public string Codice { get; set; }
        public string Versione { get; set; }
        public string Descrizione { get; set; }
        public DateTime? DataInizio { get; set; }
        public string Stato { get; set; }   
    }
}