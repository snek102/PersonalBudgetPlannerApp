
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using PersonalBudgetPlannerApp.Data;
using PersonalBudgetPlannerApp.Models;
using System.Collections.Generic;

namespace PersonalBudgetPlannerApp.Controllers{
    public class ExpenseController : Controller{
        private readonly DatabaseHelper _dbHelper;

        public ExpenseController(DatabaseHelper dbHelper){
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            List<Expense> expenses = _dbHelper.GetExpenses();
            return View(expenses);
        }

        public IActionResult Create()
        {
      
            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Amount,Description,ExpenseDate,CategoryId")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dbHelper.AddExpense(expense);
                    TempData["SuccessMessage"] = "Expense added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the expense: " + ex.Message);
                }
            }
            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", expense.CategoryId);
            return View(expense);
        }

        
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = _dbHelper.GetExpenseById(id.Value);
            if (expense == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", expense.CategoryId);
            return View(expense);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Amount,Description,ExpenseDate,CategoryId")] Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbHelper.UpdateExpense(expense);
                    TempData["SuccessMessage"] = "Expense updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the expense: " + ex.Message);
                }
            }
            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", expense.CategoryId);
            return View(expense);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = _dbHelper.GetExpenseById(id.Value);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _dbHelper.DeleteExpense(id);
                TempData["SuccessMessage"] = "Expense deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting expense: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}