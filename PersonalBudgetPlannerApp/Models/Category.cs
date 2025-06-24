
using System.ComponentModel.DataAnnotations; 

namespace PersonalBudgetPlannerApp.Models
{
    public class Category
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters.")]
        public string Name { get; set; } 
    }
}