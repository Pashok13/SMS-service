﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Model.ViewModels.OperatorViewModels;
using BAL.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using WebApp.Models;
using Model.Interfaces;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OperatorController : Controller
    {

        private readonly IOperatorManager operatorManager;

        public OperatorController(IOperatorManager oper)
        {
            this.operatorManager = oper;
        }
        
        /// <summary>
        /// Get Operator input form and list
        /// </summary>
        [HttpGet]
        public IActionResult Operators(PageState pageState)
        {
            if (ModelState.IsValid)
            {
                var page = operatorManager.GetPage(pageState);

                ViewBag.Operators = page.OperatorList;
                ViewBag.PageState = page.PageState;
                return View();
            }
            TempData["ErrorMessage"] = "Internal error";
            return RedirectToAction("Operators", "Operator");
        }

        /// <summary>
        /// Add new Operator to DB
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Operators(OperatorViewModel newOper, PageState pageState)
        {
            if (ModelState.IsValid)
            {
                var result = operatorManager.Add(newOper);
                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Details;
                    return Redirect(Url.Action("Operators", pageState));
                }
                else
                {
                    return Redirect(Url.Action("Operators", pageState));
                }
            }
            TempData["ErrorMessage"] = "Internal error!";
            return RedirectToAction("Operators", "Operator");
        }

        /// <summary>
        /// Remove operator with specified Id. Uses pageStateJson for simplicity
        /// </summary>
        public IActionResult Remove(int operatorId, string pageStateJson)
        {
            PageState pageState = JsonConvert.DeserializeObject<PageState>(pageStateJson);
            var result = operatorManager.Remove(operatorId);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Details;
                return Redirect(Url.Action("Operators", pageState));
            }
            else
            {
                return Redirect(Url.Action("Operators", pageState));
            }
        }

        /// <summary>
        /// Edit operator
        /// </summary>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Operator(OperatorViewModel editedOper, PageState pageState)
        {
            if (ModelState.IsValid)
            {
                var result = operatorManager.Update(editedOper);
                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Details;
                    return Redirect(Url.Action("Operators", pageState));
                }
                else
                {
                    return Redirect(Url.Action("Operators", pageState));
                }
            }
            TempData["ErrorMessage"] = "Internal error!";
            return RedirectToAction("Operators", "Operator");
        }

        public IActionResult NextPage(PageState pageState)
        {
            pageState.Page++;
            return Redirect(Url.Action("Operators", pageState));
        }

        public IActionResult PreviousPage(PageState pageState)
        {
            pageState.Page--;
            return Redirect(Url.Action("Operators", pageState));
        }

        /// <summary>
        /// Got to specified page. Uses pageStateJson for simplicity
        /// </summary>
        public IActionResult SelectPage(int page, string pageStateJson)
        {
            PageState pageState = JsonConvert.DeserializeObject<PageState>(pageStateJson);
            pageState.Page = page;
            return Redirect(Url.Action("Operators", pageState));
        }

        /// <summary>
        /// Search operator with specified querry
        /// </summary>
        [HttpPost]
        public IActionResult SearchOperators(OperatorSearchViewModel search, PageState pageState)
        {
            if (ModelState.IsValid)
            {
                pageState.SearchQuerry = search.SearchQuerry ?? "";
                return Redirect(Url.Action("Operators", pageState));
            }
            TempData["ErrorMessage"] = "Internal error";
            return RedirectToAction("Operators", "Operator");
        }

        /// <summary>
        /// Redirect to code editor for Operator with specfied Id. Uses TempData for id transmission
        /// </summary>        
        public IActionResult EditCodes(int id)
        {
            TempData["OperatorId"] = id;
            return RedirectToAction("Codes", "Code");
        }

        [HttpGet]
        public IActionResult AddLogo(int operatorId)
        {
            return View(new LogoViewModel() { OperatorId = operatorId });
        }

        [HttpPost]
        public IActionResult AddLogo(LogoViewModel logo)
        {
            if (ModelState.IsValid)
            {
                var result = operatorManager.AddLogo(logo);
                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Details;
                    return RedirectToAction("Operators", "Operator");
                }
                else
                {
                    return RedirectToAction("Operators", "Operator");
                }
            }
            TempData["ErrorMessage"] = "Internal error";
            return RedirectToAction("Operators", "Operator");
        }

    }
}
