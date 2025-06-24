using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PersonalBudgetPlannerApp.Models
{
    public class ExpenseViewModel
    {
        public Expense Expense { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
