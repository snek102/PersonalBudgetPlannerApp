
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using PersonalBudgetPlannerApp.Data;
using PersonalBudgetPlannerApp.Models;
using System.Collections.Generic;

namespace PersonalBudgetPlannerApp.Controllers
{
    public class IncomeController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public IncomeController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

  
        public IActionResult Index()
        {
            List<Income> incomes = _dbHelper.GetIncomes();
            return View(incomes);
        }

        
        public IActionResult Create()
        {
           
            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Amount,Description,IncomeDate,CategoryId")] Income income)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dbHelper.AddIncome(income);
                    TempData["SuccessMessage"] = "Income added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the income: " + ex.Message);
                }
            }
            
            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", income.CategoryId);
            return View(income);
        }

        
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = _dbHelper.GetIncomeById(id.Value);
            if (income == null)
            {
                return NotFound();
            }
            
            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", income.CategoryId);
            return View(income);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Amount,Description,IncomeDate,CategoryId")] Income income)
        {
            if (id != income.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbHelper.UpdateIncome(income);
                    TempData["SuccessMessage"] = "Income updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the income: " + ex.Message);
                }
            }
            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", income.CategoryId);
            return View(income);
        }

        
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = _dbHelper.GetIncomeById(id.Value);
            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _dbHelper.DeleteIncome(id);
                TempData["SuccessMessage"] = "Income deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting income: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}