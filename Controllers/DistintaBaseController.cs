using System;
using System.Collections.Generic;
using System.Linq;
using Linea_Produzione.Models;
using Microsoft.AspNetCore.Mvc;

namespace Linea_Produzione.Controllers
{
    public class DistintaBaseController : Controller
    {
        // Questo risponde a: /DistintaBase/DistintaBase
        public IActionResult DistintaBase()
        {
            var searchModel = new DistintaBaseSearchModel();
            return View(searchModel);
        }

        // Questo gestisce la ricerca AJAX
        [HttpGet]
        public IActionResult Ricerca(DistintaBaseSearchModel search)
        {
            // Dati finti per il test
            var archivio = new List<DistintaBaseGridModel>
            {
                new DistintaBaseGridModel { Id = 1, Codice = "DB-001", Descrizione = "Distinta Base Scocca Alfa", DataInizio = new DateTime(2026, 1, 10), Versione = "1.0", Stato = "Approvato" },
                new DistintaBaseGridModel { Id = 2, Codice = "DB-002", Descrizione = "Assieme Motore Elettrico X", DataInizio = new DateTime(2026, 3, 15), Versione = "2.1", Stato = "In Sviluppo" },
                new DistintaBaseGridModel { Id = 3, Codice = "DB-003", Descrizione = "Cablaggio Principale L1", DataInizio = new DateTime(2026, 5, 20), Versione = "1.2", Stato = "Approvato" }
            };

            var query = archivio.AsQueryable();

            if (!string.IsNullOrEmpty(search.Codice))
            {
                query = query.Where(d => d.Codice.Contains(search.Codice, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(search.Descrizione))
            {
                query = query.Where(d => d.Descrizione.Contains(search.Descrizione, StringComparison.OrdinalIgnoreCase));
            }
            if (search.DataInizio.HasValue)
            {
                query = query.Where(d => d.DataInizio == search.DataInizio.Value.Date);
            }

            return Json(query.ToList());
        }

        // Controllers/DistintaBaseController.cs

        [HttpGet]
        public IActionResult Riepilogo(int id)
        {
            // 1. Recupera la distinta dal DB usando l'id (qui simulato)
            // var distinta = _context.DistinteBase.Find(id);

            // Dati finti per il test
            var archivio = new List<DistintaBaseGridModel>
            {
                new DistintaBaseGridModel { Id = 1, Codice = "DB-001", Descrizione = "Distinta Base Scocca Alfa", DataInizio = new DateTime(2026, 1, 10), Versione = "1.0", Stato = "Approvato" },
                new DistintaBaseGridModel { Id = 2, Codice = "DB-002", Descrizione = "Assieme Motore Elettrico X", DataInizio = new DateTime(2026, 3, 15), Versione = "2.1", Stato = "In Sviluppo" },
                new DistintaBaseGridModel { Id = 3, Codice = "DB-003", Descrizione = "Cablaggio Principale L1", DataInizio = new DateTime(2026, 5, 20), Versione = "1.2", Stato = "Approvato" }
            };

            var query = archivio.Where(e => e.Id == id).FirstOrDefault();

            // 2. Mappatura sul modello Edit richiesto
            var model = new DistintaBaseModel
            {
                Id = query.Id,
                Codice = query.Codice,
                Descrizione = query.Descrizione,
                DataInizio = query.DataInizio,
                Versione = query.Versione,
                Stato = query.Stato,
                ReadOnly = true
            };

            // Ritorniamo una PartialView passandogli il modello
            return PartialView("DistintaBaseEditModel", model);
        }
    }
}