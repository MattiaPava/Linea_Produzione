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

        // Aggiungi questo metodo in fondo alla classe BaseManager
        public bool SalvaNuovaDistinta(DistintaBaseModel model)
        {
            try
            {
                // Mappiamo il modello proveniente dalla View verso l'oggetto reale del DB
                var nuovaEntita = new DistintaBase // Sostituisci con la classe reale mappata su EF se diversa
                {
                    Id = model.Id,
                    IdUtente = model.IdUtente,
                    Codice = model.Codice,
                    Descrizione = model.Descrizione,
                    DataInizio = model.DataInizio,
                    Versione = model.Versione,
                    Stato = 1 // Stato iniziale automatico
                };

                _context.DistinteBase.Add(nuovaEntita);

                // Eseguiamo il COMMIT effettivo sul Database
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                // Qui puoi loggare l'errore se necessario
                return false;
            }
        }
    }
}