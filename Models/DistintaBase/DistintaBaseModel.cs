using System;
using System.ComponentModel.DataAnnotations;

// SOLUZIONE: Disabilita il controllo severo dei null solo in questo file.
// In questo modo 'string' torna a poter essere null senza far arrabbiare il validatore.
#nullable disable

namespace Linea_Produzione.Models
{
    public class DistintaBaseModel
    {
        [Key]
        public long Id { get; set; } // IDENTITY (1, 1) NOT NULL

        [Required(ErrorMessage = "L'utente è obbligatorio.")]
        public long IdUtente { get; set; } // BIGINT NOT NULL

        [Required(ErrorMessage = "Il codice è obbligatorio.")]
        [StringLength(12, ErrorMessage = "Il codice non può superare i 12 caratteri.")]
        public string Codice { get; set; } // NVARCHAR (12) NOT NULL

        // NESSUN [Required] perché sul DB è NULL
        [StringLength(512, ErrorMessage = "La descrizione non può superare i 512 caratteri.")]
        public string Descrizione { get; set; } // NVARCHAR (512) NULL

        [Required(ErrorMessage = "La data di inizio è obbligatoria.")]
        public DateTime DataInizio { get; set; } // DATETIME2 (7) NOT NULL

        [Required(ErrorMessage = "La versione è obbligatoria.")]
        [StringLength(12, ErrorMessage = "La versione non può superare i 12 caratteri.")]
        public string Versione { get; set; } // NVARCHAR (12) NOT NULL

        // NESSUN [Required] perché sul DB è NULL. 
        // Grazie a '#nullable disable' viaggerà come null senza rompere il ModelState.IsValid!
        [StringLength(64, ErrorMessage = "Lo stato non può superare i 64 caratteri.")]
        public string Stato { get; set; } // NVARCHAR (64) NULL

        public bool ReadOnly { get; set; }
    }
}