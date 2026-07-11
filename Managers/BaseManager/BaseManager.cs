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
    }
}