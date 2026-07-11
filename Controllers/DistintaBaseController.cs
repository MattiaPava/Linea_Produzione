using Humanizer.Localisation;
using Linea_Produzione.Managers.BaseManager;
using Linea_Produzione.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // 2. Gestisce la ricerca AJAX sulla griglia - MODIFICATO CON MAPPING PULITO
        [HttpGet]
        public IActionResult Ricerca(DistintaBaseSearchModel search)
        {
            var risultato = _baseManager.CercaDistintaBase(search);

            // Applica il mapping per convertire l'Enum e formattare le proprietà prima dell'invio
            var risultatoMappato = MapToGridModel(risultato);

            return Json(risultatoMappato);
        }

        // 4. Inizializza il modello per la creazione e restituisce la Partial View
        [HttpGet]
        public IActionResult Nuovo(long idUtente)
        {
            var model = new DistintaBaseModel
            {
                IdUtente = idUtente,
                Id = 0,
                DataInizio = DateTime.Today,
                Versione = "01",
                Stato = StatoDistinta.Created,
                ReadOnly = false,
                ShowCloseButton = true
            };

            ViewBag.StatoOptions = GetStatoSelectList(model.Stato, isNuovo: true);

            return PartialView("DistintaBaseEditModel", model);
        }

        // NEW. Carica la maschera in modalità MODIFICA (Campi editabili, Stato gestito)
        [HttpGet]
        public IActionResult Modifica(long id)
        {
            var searchModel = new DistintaBaseSearchModel { Id = id };
            var distinta = _baseManager.CercaDistintaBase(searchModel).FirstOrDefault();

            if (distinta == null)
            {
                return NotFound("Distinta base non trovata.");
            }

            var model = new DistintaBaseModel
            {
                Id = distinta.Id,
                IdUtente = distinta.IdUtente,
                Codice = distinta.Codice,
                Descrizione = distinta.Descrizione,
                DataInizio = distinta.DataInizio,
                Versione = distinta.Versione,
                Stato = (StatoDistinta)distinta.Stato,
                ReadOnly = false,
                ShowCloseButton = true
            };

            ViewBag.StatoOptions = GetStatoSelectList(model.Stato, isNuovo: false);

            return PartialView("DistintaBaseEditModel", model);
        }

        // 3. Carica il Riepilogo parziale in sola lettura
        [HttpGet]
        public IActionResult Riepilogo(long id)
        {
            var searchModel = new DistintaBaseSearchModel { Id = id };
            var distinta = _baseManager.CercaDistintaBase(searchModel).FirstOrDefault();

            if (distinta == null)
            {
                return NotFound("Distinta base non trovata.");
            }

            var model = new DistintaBaseModel
            {
                Id = distinta.Id,
                IdUtente = distinta.IdUtente,
                Codice = distinta.Codice,
                Descrizione = distinta.Descrizione,
                DataInizio = distinta.DataInizio,
                Versione = distinta.Versione,
                Stato = (StatoDistinta)distinta.Stato,
                ReadOnly = true,
                ShowCloseButton = false
            };

            ViewBag.StatoOptions = GetStatoSelectList(model.Stato, isNuovo: false);

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

            bool esito = _baseManager.SalvaDistintaBase(model);

            if (esito)
            {
                return Json(new { success = true, message = "Distinta Base salvata correttamente!" });
            }

            return Json(new { success = false, message = "Errore durante il salvataggio a database." });
        }

        #region Helper Methods

        /// <summary>
        /// NUOVO METODO: Mappa l'output grezzo in un oggetto anonimo pulito.
        /// Converte l'Enum direttamente nella stringa tradotta dal file .resx.
        /// </summary>
        private IEnumerable<object> MapToGridModel(IEnumerable<dynamic> listaRaw)
        {
            if (listaRaw == null) return Enumerable.Empty<object>();

            return listaRaw.Select(e => {
                // Cast di sicurezza del valore del DB nell'Enum strutturato
                StatoDistinta statoEnum = (StatoDistinta)e.Stato;

                // Recupero del testo localizzato dal file di risorse .resx
                string resourceKey = $"Stato_{statoEnum}";
                string statoTradotto = Resources.AppResources.ResourceManager.GetString(resourceKey) ?? statoEnum.ToString();

                // Generazione del JSON finale pulito, privo di ID o codici numerici residui per lo stato
                return new
                {
                    Id = e.Id,
                    Codice = e.Codice,
                    Descrizione = e.Descrizione,
                    Versione = e.Versione,
                    DataInizio = e.DataInizio.ToString("dd/MM/yyyy"), // Formattazione nativa della data
                    Stato = statoTradotto                             // Diventa direttamente "Creato", "In Definizione", ecc.
                };
            }).ToList();
        }

        /// <summary>
        /// Genera la lista di opzioni localizzate per l'HTML Select dello Stato, applicando le regole di business.
        /// </summary>
        private List<SelectListItem> GetStatoSelectList(StatoDistinta statoAttuale, bool isNuovo)
        {
            var list = new List<SelectListItem>();

            if (isNuovo)
            {
                list.Add(new SelectListItem
                {
                    Value = ((short)StatoDistinta.Created).ToString(),
                    Text = Resources.AppResources.Stato_Created,
                    Selected = true
                });
            }
            else
            {
                foreach (StatoDistinta st in Enum.GetValues(typeof(StatoDistinta)))
                {
                    if (statoAttuale == StatoDistinta.Modified && st != StatoDistinta.Modified && st != StatoDistinta.Archived)
                    {
                        continue;
                    }

                    if (statoAttuale == StatoDistinta.Archived && st != StatoDistinta.Archived)
                    {
                        continue;
                    }

                    string resourceKey = $"Stato_{st}";
                    string localizedText = Resources.AppResources.ResourceManager.GetString(resourceKey) ?? st.ToString();

                    list.Add(new SelectListItem
                    {
                        Value = ((short)st).ToString(),
                        Text = localizedText,
                        Selected = (st == statoAttuale)
                    });
                }
            }

            return list;
        }

        #endregion
    }
}