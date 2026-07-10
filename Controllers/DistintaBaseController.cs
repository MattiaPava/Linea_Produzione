using System;
using System.Linq;
using Linea_Produzione.Managers.BaseManager;
using Linea_Produzione.Models;
using Microsoft.AspNetCore.Mvc;

namespace Linea_Produzione.Controllers
{
    public class DistintaBaseController : Controller
    {
        private readonly IBaseManager _baseManager;

        public DistintaBaseController(IBaseManager baseManager)
        {
            _baseManager = baseManager;
        }

        // 1. Risponde a: /DistintaBase/DistintaBase
        public IActionResult DistintaBase()
        {
            long idUtenteCorrente = 1;

            var searchModel = new DistintaBaseSearchModel
            {
                IdUtente = idUtenteCorrente
            };

            return View(searchModel);
        }

        // 2. Gestisce la ricerca AJAX sulla griglia
        [HttpGet]
        public IActionResult Ricerca(DistintaBaseSearchModel search)
        {
            var risultato = _baseManager.CercaDistintaBase(search);
            return Json(risultato);
        }

        // 4. Inizializza il modello per la creazione e restituisce la Partial View
        [HttpGet]
        public IActionResult Nuovo(long idUtente)
        {
            var model = new DistintaBaseModel
            {
                IdUtente = idUtente,
                Id = 0,
                DataInizio = DateTime.Today, // Impostiamo la data di oggi come default
                Versione = "01",            // Primo rilascio di default
                ReadOnly = false            // Sblocca i campi in scrittura!
            };

            return PartialView("DistintaBaseEditModel", model);
        }

        // 5. Riceve i dati dal Popup ed esegue il salvataggio sul DB passando dal Manager
        [HttpPost]
        public IActionResult Salva(DistintaBaseModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "I dati inseriti non sono validi." });
            }

            // Invochiamo il manager per scrivere fisicamente sul database
            bool esito = _baseManager.SalvaNuovaDistinta(model);

            if (esito)
            {
                return Json(new { success = true, message = "Distinta Base salvata correttamente!" });
            }

            return Json(new { success = false, message = "Errore durante il salvataggio a database." });
        }

        // 3. Carica il Riepilogo parziale usando SEMPRE CercaDistintaBase
        [HttpGet]
        public IActionResult Riepilogo(long id)
        {
            // Istanziamo il modello di ricerca configurando solo l'ID richiesto
            var searchModel = new DistintaBaseSearchModel { Id = id };

            // Invochiamo l'unica funzione del manager ed estraiamo il primo risultato
            var distinta = _baseManager.CercaDistintaBase(searchModel).FirstOrDefault();

            if (distinta == null)
            {
                return NotFound("Distinta base non trovata.");
            }

            // Mappatura finale per la Partial View originaria
            var model = new DistintaBaseModel
            {
                Id = distinta.Id,
                Codice = distinta.Codice,
                Descrizione = distinta.Descrizione,
                DataInizio = distinta.DataInizio,
                Versione = distinta.Versione,
                Stato = distinta.Stato,
                ReadOnly = true
            };

            return PartialView("DistintaBaseEditModel", model);
        }
    }
}