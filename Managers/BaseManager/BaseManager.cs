using System;
using System.Collections.Generic;
using System.Linq;
using Linea_Produzione.Data;
using Linea_Produzione.Models;

namespace Linea_Produzione.Managers.BaseManager
{
    public class BaseManager : IBaseManager
    {
        private readonly ApplicationDbContext _context;

        public BaseManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<DistintaBaseEntity> CercaDistintaBase(DistintaBaseSearchModel search)
        {
            if (search == null) return new List<DistintaBaseEntity>();

            var query = _context.DistinteBase.AsQueryable();

            // SE COINVOLGE UN ID SPECIFICO (caso del Riepilogo): cerca direttamente la chiave primaria
            if (search.Id.HasValue && search.Id.Value > 0)
            {
                query = query.Where(d => d.Id == search.Id.Value);
            }
            else
            {
                // ALTRIMENTI: Applica il normale comportamento dei filtri della Sidebar
                query = query.Where(d => d.IdUtente == search.IdUtente);

                if (!string.IsNullOrEmpty(search.Codice))
                {
                    query = query.Where(d => d.Codice.Contains(search.Codice));
                }

                if (!string.IsNullOrEmpty(search.Descrizione))
                {
                    query = query.Where(d => d.Descrizione.Contains(search.Descrizione));
                }

                if (search.DataInizio.HasValue)
                {
                    query = query.Where(d => d.DataInizio.Date == search.DataInizio.Value.Date);
                }
            }

            // Proiezione finale nell'entità agganciata alla View
            return query.Select(d => new DistintaBaseEntity
            {
                Id = d.Id,
                IdUtente = d.IdUtente,
                Codice = d.Codice,
                Descrizione = d.Descrizione,
                DataInizio = d.DataInizio,
                Versione = d.Versione,
                Stato = d.Stato
            }).ToList();
        }

        public bool SalvaDistintaBase(DistintaBaseModel model)
        {
            try
            {
                if (model.Id == 0)
                {
                    // ==========================================
                    // LOGICA DI INSERIMENTO (NUOVA DISTINTA)
                    // ==========================================
                    var nuovaEntita = new DistintaBase
                    {
                        // NOTA: Non mappiamo l'Id. Essendo a 0, lasciamo che SQL Server 
                        // lo generi automaticamente tramite la proprietà IDENTITY.
                        IdUtente = model.IdUtente,
                        Codice = model.Codice,
                        Descrizione = model.Descrizione,
                        DataInizio = model.DataInizio,
                        Versione = model.Versione,
                        Stato = (short)model.Stato
                    };

                    _context.DistinteBase.Add(nuovaEntita);
                }
                else
                {
                    // ==========================================
                    // LOGICA DI MODIFICA (DISTINTA ESISTENTE)
                    // ==========================================
                    // Recuperiamo il record reale dal database per permettere a EF di tracciarlo
                    var entitaEsistente = _context.DistinteBase.Find(model.Id);

                    if (entitaEsistente == null)
                    {
                        // Se per qualche motivo il record è stato eliminato nel frattempo
                        return false;
                    }

                    // Aggiorniamo solo i campi modificabili dell'entità tracciata
                    entitaEsistente.IdUtente = model.IdUtente;
                    entitaEsistente.Codice = model.Codice;
                    entitaEsistente.Descrizione = model.Descrizione;
                    entitaEsistente.DataInizio = model.DataInizio;
                    entitaEsistente.Versione = model.Versione;

                    // NOTA: Lo Stato in fase di modifica di solito non si forza a 1, 
                    // ma mantiene quello attuale sul DB. Se la tua vista gestisce lo stato,
                    // puoi scommentare la riga sotto:
                    entitaEsistente.Stato = (short)model.Stato;
                }

                // Il SaveChanges invierà al DB una INSERT o una UPDATE a seconda del flusso eseguito
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Se hai un logger (es. ILogger<BaseManager>), puoi usarlo qui:
                // _logger.LogError(ex, "Errore durante il salvataggio della distinta base");

                return false;
            }
        }
    }
}