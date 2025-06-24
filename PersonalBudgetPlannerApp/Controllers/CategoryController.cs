using Microsoft.AspNetCore.Mvc;
using PersonalBudgetPlannerApp.Data;
using PersonalBudgetPlannerApp.Models; 
using System.Collections.Generic; 
using Microsoft.Data.SqlClient; 

namespace PersonalBudgetPlannerApp.Controllers{
    public class CategoryController : Controller{
        private readonly DatabaseHelper _dbHelper;
        public CategoryController(DatabaseHelper dbHelper){
            _dbHelper = dbHelper;
        }
        public IActionResult Index(){
            List<Category> categories = _dbHelper.GetCategories(); 
            return View(categories); 
        }

        public IActionResult Create(){
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult Create([Bind("Name")] Category category){
            if (ModelState.IsValid) {
                try{
                    _dbHelper.AddCategory(category); 
                    TempData["SuccessMessage"] = "Category added successfully!";
                    return RedirectToAction(nameof(Index)); 
                }
                catch (SqlException ex) {
                    
                    if (ex.Number == 2601 || ex.Number == 2627){
                        ModelState.AddModelError("Name", "A category with this name already exists."); 
                    }
                    else{
                        ModelState.AddModelError("", "An error occurred while saving the category: " + ex.Message); 
                    }
                    return View(category); 
                }
                catch (Exception ex) {
                    ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
                    return View(category);
                }
            }
            return View(category); 
        }

        public IActionResult Edit(int? id) {
            if (id == null){
                return NotFound(); 
            }

            var category = _dbHelper.GetCategoryById(id.Value); 
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id) {
                return NotFound();
            }

            if (ModelState.IsValid){
                try{
                    _dbHelper.UpdateCategory(category);
                    TempData["SuccessMessage"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex){
                    if (ex.Number == 2601 || ex.Number == 2627){
                        ModelState.AddModelError("Name", "A category with this name already exists.");
                    }
                    else{
                        ModelState.AddModelError("", "An error occurred while updating the category: " + ex.Message);
                    }
                    return View(category);
                }
                catch (Exception ex){
                    ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
                    return View(category);
                }
            }
            return View(category);
        }
        public IActionResult Delete(int? id){
            if (id == null){
                return NotFound();
            }

            var category = _dbHelper.GetCategoryById(id.Value);
            if (category == null){
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id){
            try{
                _dbHelper.DeleteCategory(id); 
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (Exception ex){
                
                TempData["ErrorMessage"] = $"Error deleting category: {ex.Message}. It might be associated with existing incomes or expenses. Please delete those first.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}