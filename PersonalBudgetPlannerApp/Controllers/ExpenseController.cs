using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PersonalBudgetPlannerApp.Data;
using PersonalBudgetPlannerApp.Models;
using System.Collections.Generic;

namespace PersonalBudgetPlannerApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ExpenseController(DatabaseHelper dbHelper)
        {
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
        public IActionResult Create(Expense expense)
        {
            // Debugging: log bound values and errors
            Console.WriteLine($"DEBUG → CategoryId = {expense.CategoryId}");
            Console.WriteLine($"DEBUG → Amount = {expense.Amount}, Description = {expense.Description}, ExpenseDate = {expense.ExpenseDate}");

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"DEBUG → ModelState Error on '{entry.Key}': {error.ErrorMessage}");
                    }
                }

                // Repopulate dropdown to avoid nulls
                ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", expense.CategoryId);
                return View(expense);
            }

            try
            {
                _dbHelper.AddExpense(expense);
                TempData["SuccessMessage"] = "Expense added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR → Exception during AddExpense: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the expense.");
            }

            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", expense.CategoryId);
            return View(expense);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var expense = _dbHelper.GetExpenseById(id.Value);
            if (expense == null) return NotFound();

            ViewBag.Categories = new SelectList(_dbHelper.GetCategories(), "Id", "Name", expense.CategoryId);
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Expense expense)
        {
            if (id != expense.Id) return NotFound();

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
            if (id == null) return NotFound();

            var expense = _dbHelper.GetExpenseById(id.Value);
            if (expense == null) return NotFound();

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
