
using System.ComponentModel.DataAnnotations; 

namespace PersonalBudgetPlannerApp.Models
{
    public class Income
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")] 
        [DataType(DataType.Currency)] 
        public decimal Amount { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; } 

        [Required(ErrorMessage = "Income Date is required.")]
        [DataType(DataType.Date)] 
        public DateTime IncomeDate { get; set; } = DateTime.Today; 

        public int? CategoryId { get; set; } 
        public Category Category { get; set; }
    }
}