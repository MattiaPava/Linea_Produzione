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

        #region DistintaBase

        public List<DistintaBaseEntity> CercaDistintaBase(DistintaBaseSearchModel searchModel)
        {
            if (searchModel == null) return new List<DistintaBaseEntity>();

            var query = _context.DistinteBase.AsQueryable();

            // 1. Filtri di Identificazione (Puntuali o per Utente)
            query = searchModel.Id.HasValue && searchModel.Id.Value > 0 ? query.Where(d => d.Id == searchModel.Id.Value) : query;
            query = searchModel.Ids != null && searchModel.Ids.Count > 0 ? query.Where(d => searchModel.Ids.Contains(d.Id)) : query;
            query = searchModel.IdUtente.HasValue ? query.Where(d => d.IdUtente == searchModel.IdUtente.Value) : query;

            // 2. Filtri della Sidebar (Applicati se valorizzati)
            query = !string.IsNullOrEmpty(searchModel.Codice) ? query.Where(d => d.Codice.Contains(searchModel.Codice)) : query;

            // Gestione Versione: Estrazione preventiva della stringa pulita ed applicazione del ternario
            string? versionePulita = !string.IsNullOrEmpty(searchModel.Versione) ? searchModel.Versione.Replace("v", "", StringComparison.OrdinalIgnoreCase).Trim() : null;
            query = !string.IsNullOrEmpty(versionePulita) ? query.Where(d => d.Versione.Contains(versionePulita)) : query;

            query = !string.IsNullOrEmpty(searchModel.Descrizione) ? query.Where(d => d.Descrizione.Contains(searchModel.Descrizione)) : query;
            query = searchModel.DataInizio.HasValue ? query.Where(d => d.DataInizio.Date == searchModel.DataInizio.Value.Date) : query;

            // 3. Filtro Stato: Switch Expression integrata perfettamente nella logica a pipeline
            query = !string.IsNullOrEmpty(searchModel.Stato) ? searchModel.Stato switch
            {
                "NonArchiviati" => query.Where(d => d.Stato != (short)StatoDistinta.Archived),
                "Archiviati" => query.Where(d => d.Stato == (short)StatoDistinta.Archived),
                "Creati" => query.Where(d => d.Stato == (short)StatoDistinta.Created),
                "Modificati" => query.Where(d => d.Stato == (short)StatoDistinta.Modified),
                "Completati" => query.Where(d => d.Stato == (short)StatoDistinta.Completed),
                _ => query // Caso "None" o default: nessun filtro applicato
            } : query;

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
                // 1. Elaborazione dell'entità (Inserimento o Modifica)
                if (!SalvaDistintaBaseInternal(model))
                {
                    return false;
                }

                // 2. Persistenza dei dati sul Database
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Errore durante il salvataggio della distinta base");
                return false;
            }
        }

        private bool SalvaDistintaBaseInternal(DistintaBaseModel model)
        {
            // 1. Validazione preventiva dei campi (sul modello già calcolato)
            if (!CheckFields(model))
            {
                return false;
            }

            // 2. Calcolo dei campi derivati e automatismi di stato
            CalculatedFields(model);

            // 3. Mappatura al modello BL
            if (model.Id == 0)
            {
                // ==========================================
                // LOGICA DI INSERIMENTO (NUOVA DISTINTA)
                // ==========================================
                var nuovaEntita = new DistintaBase
                {
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
                var entitaEsistente = _context.DistinteBase.Find(model.Id);

                if (entitaEsistente == null)
                {
                    return false; // Il record non esiste più sul DB
                }

                // Aggiornamento dei campi tracciati da EF
                entitaEsistente.IdUtente = model.IdUtente;
                entitaEsistente.Codice = model.Codice;
                entitaEsistente.Descrizione = model.Descrizione;
                entitaEsistente.DataInizio = model.DataInizio;
                entitaEsistente.Versione = model.Versione;
                entitaEsistente.Stato = (short)model.Stato;
            }

            return true;
        }

        private void CalculatedFields(DistintaBaseModel model)
        {
            // Se l'Id è diverso da 0 siamo in modifica. 
            // Se lo stato è ancora "Created", lo facciamo avanzare automaticamente a "Modified".
            if (model.Id != 0 && model.Stato == StatoDistinta.Created)
            {
                model.Stato = StatoDistinta.Modified;
            }
        }

        private bool CheckFields(DistintaBaseModel model)
        {
            // 1. Controllo "Between" sullo Stato (castato a short/int per verificare il range)
            // Modifica i valori min (es. 0) e max (es. 3) in base alla tua reale Enum logica
            int statoValore = (int)model.Stato;
            if (statoValore < 0 || statoValore > 3)
            {
                return false;
            }

            // 2. Esempio di altri controlli di pulizia/validazione utili in backend
            //if (string.IsNullOrWhiteSpace(model.Codice))
            //{
            //    return false;
            //}

            return true;
        }

        public bool CancellaDistintaBase(long id)
        {
            // Invoca la funzione di cancellazione interna che marca il record come rimosso
            bool success = CancellaDistintaBaseInternal(id);

            // Se l'entità non è stata trovata o contrassegnata, usciamo subito senza toccare il DB
            if (!success) return false;

            // Esegue la persistenza sul database e verifica che sia stata modificata almeno una riga
            return _context.SaveChanges() > 0;
        }

        private bool CancellaDistintaBaseInternal(long id)
        {
            var distinta = _context.DistinteBase.Find(id);

            // Se il record non esiste o è già stato rimosso, restituisce false
            if (distinta == null) return false;

            // Mette l'entità in stato "Deleted" all'interno del contesto (Context Tracking)
            _context.DistinteBase.Remove(distinta);

            // Restituisce true se l'operazione di preparazione alla cancellazione è andata a buon fine
            return true;
        }

        #endregion DistintaBase

    }
}