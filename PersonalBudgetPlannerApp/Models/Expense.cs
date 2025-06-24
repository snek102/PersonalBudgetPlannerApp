
using System.ComponentModel.DataAnnotations; 

namespace PersonalBudgetPlannerApp.Models
{
    public class Expense
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")] 
        [DataType(DataType.Currency)] 
        public decimal Amount { get; set; } 

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; } 

        [Required(ErrorMessage = "Expense Date is required.")]
        [DataType(DataType.Date)] 
        public DateTime ExpenseDate { get; set; } = DateTime.Today; 

        [Required(ErrorMessage = "Category is required.")] 
        public int? CategoryId { get; set; } 
        
        public Category Category { get; set; }
    }
}