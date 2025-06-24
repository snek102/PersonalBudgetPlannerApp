
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetPlannerApp.Data; 
using PersonalBudgetPlannerApp.Models; 
using System.Diagnostics;
using System.Linq; 

namespace PersonalBudgetPlannerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _dbHelper; 
        private readonly ILogger<HomeController> _logger; 

        public HomeController(ILogger<HomeController> logger, DatabaseHelper dbHelper)
        {
            _logger = logger;
            _dbHelper = dbHelper; 
        }

        
        public IActionResult Index()
        {
            
            ViewBag.TotalIncome = _dbHelper.GetTotalIncome();
            ViewBag.TotalExpenses = _dbHelper.GetTotalExpenses();
            ViewBag.Balance = ViewBag.TotalIncome - ViewBag.TotalExpenses;

            ViewBag.RecentIncomes = _dbHelper.GetIncomes().Take(5).ToList(); 
            ViewBag.RecentExpenses = _dbHelper.GetExpenses().Take(5).ToList(); 

            return View(); 
        }

       
        public IActionResult Privacy()
        {
            return View();
        }

     
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}