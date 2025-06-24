
using Microsoft.Data.SqlClient; 
using PersonalBudgetPlannerApp.Models; 
using System.Data; 
using System.Collections.Generic; // For List<T>
using Microsoft.Extensions.Configuration; 
using System;

namespace PersonalBudgetPlannerApp.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();
        
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Name FROM Categories ORDER BY Name";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open(); 
                    using (SqlDataReader rdr = cmd.ExecuteReader()) 
                    {
                        while (rdr.Read()) 
                        {
                            categories.Add(new Category
                            {
                                Id = Convert.ToInt32(rdr["Id"]), 
                                Name = rdr["Name"].ToString()   
                            });
                        }
                    }
                }
            }
            return categories;
        }

      
        public void AddCategory(Category category)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Categories (Name) VALUES (@Name)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    
                    cmd.Parameters.AddWithValue("@Name", category.Name);
                    con.Open();
                    cmd.ExecuteNonQuery(); 
                }
            }
        }


        public Category GetCategoryById(int id)
        {
            Category category = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Name FROM Categories WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read()) 
                        {
                            category = new Category
                            {
                                Id = Convert.ToInt32(rdr["Id"]),
                                Name = rdr["Name"].ToString()
                            };
                        }
                    }
                }
            }
            return category;
        }

        public void UpdateCategory(Category category)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Categories SET Name = @Name WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", category.Name);
                    cmd.Parameters.AddWithValue("@Id", category.Id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCategory(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open(); 
                SqlTransaction transaction = con.BeginTransaction(); 

                try
                {
                   
                    string deleteExpensesQuery = "DELETE FROM Expenses WHERE CategoryId = @Id";
                    using (SqlCommand cmd = new SqlCommand(deleteExpensesQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

           
                    string deleteIncomesQuery = "DELETE FROM Incomes WHERE CategoryId = @Id";
                    using (SqlCommand cmd = new SqlCommand(deleteIncomesQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    
                    string deleteCategoryQuery = "DELETE FROM Categories WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(deleteCategoryQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; 
                }
            }
        }

        public List<Income> GetIncomes()
        {
            List<Income> incomes = new List<Income>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT i.Id, i.Amount, i.Description, i.IncomeDate, i.CategoryId, c.Name AS CategoryName
                    FROM Incomes i
                    LEFT JOIN Categories c ON i.CategoryId = c.Id
                    ORDER BY i.IncomeDate DESC"; 
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            incomes.Add(new Income
                            {
                                Id = Convert.ToInt32(rdr["Id"]),
                                Amount = Convert.ToDecimal(rdr["Amount"]),
                                Description = rdr["Description"] as string, 
                                IncomeDate = Convert.ToDateTime(rdr["IncomeDate"]),
                              
                                CategoryId = rdr["CategoryId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["CategoryId"]),
                                Category = rdr["CategoryName"] == DBNull.Value ? null : new Category { Id = rdr["CategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["CategoryId"]), Name = rdr["CategoryName"].ToString() }
                            });
                        }
                    }
                }
            }
            return incomes;
        }

        public void AddIncome(Income income)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Incomes (Amount, Description, IncomeDate, CategoryId) VALUES (@Amount, @Description, @IncomeDate, @CategoryId)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Amount", income.Amount);
                  
                    cmd.Parameters.AddWithValue("@Description", (object)income.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IncomeDate", income.IncomeDate);
                    cmd.Parameters.AddWithValue("@CategoryId", (object)income.CategoryId ?? DBNull.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Income GetIncomeById(int id)
        {
            Income income = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT i.Id, i.Amount, i.Description, i.IncomeDate, i.CategoryId, c.Name AS CategoryName
                    FROM Incomes i
                    LEFT JOIN Categories c ON i.CategoryId = c.Id
                    WHERE i.Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            income = new Income
                            {
                                Id = Convert.ToInt32(rdr["Id"]),
                                Amount = Convert.ToDecimal(rdr["Amount"]),
                                Description = rdr["Description"] as string,
                                IncomeDate = Convert.ToDateTime(rdr["IncomeDate"]),
                                CategoryId = rdr["CategoryId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["CategoryId"]),
                                Category = rdr["CategoryName"] == DBNull.Value ? null : new Category { Id = rdr["CategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["CategoryId"]), Name = rdr["CategoryName"].ToString() }
                            };
                        }
                    }
                }
            }
            return income;
        }

        public void UpdateIncome(Income income)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Incomes SET Amount = @Amount, Description = @Description, IncomeDate = @IncomeDate, CategoryId = @CategoryId WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Amount", income.Amount);
                    cmd.Parameters.AddWithValue("@Description", (object)income.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IncomeDate", income.IncomeDate);
                    cmd.Parameters.AddWithValue("@CategoryId", (object)income.CategoryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", income.Id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteIncome(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Incomes WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<Expense> GetExpenses()
        {
            List<Expense> expenses = new List<Expense>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT e.Id, e.Amount, e.Description, e.ExpenseDate, e.CategoryId, c.Name AS CategoryName
                    FROM Expenses e
                    JOIN Categories c ON e.CategoryId = c.Id
                    ORDER BY e.ExpenseDate DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            expenses.Add(new Expense
                            {
                                Id = Convert.ToInt32(rdr["Id"]),
                                Amount = Convert.ToDecimal(rdr["Amount"]),
                                Description = rdr["Description"] as string,
                                ExpenseDate = Convert.ToDateTime(rdr["ExpenseDate"]),
                                CategoryId = Convert.ToInt32(rdr["CategoryId"]),
                                Category = new Category { Id = Convert.ToInt32(rdr["CategoryId"]), Name = rdr["CategoryName"].ToString() }
                            });
                        }
                    }
                }
            }
            return expenses;
        }

        public void AddExpense(Expense expense)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Expenses (Amount, Description, ExpenseDate, CategoryId) VALUES (@Amount, @Description, @ExpenseDate, @CategoryId)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Amount", expense.Amount);
                    cmd.Parameters.AddWithValue("@Description", (object)expense.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExpenseDate", expense.ExpenseDate);
                    cmd.Parameters.AddWithValue("@CategoryId", expense.CategoryId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Expense GetExpenseById(int id)
        {
            Expense expense = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT e.Id, e.Amount, e.Description, e.ExpenseDate, e.CategoryId, c.Name AS CategoryName
                    FROM Expenses e
                    JOIN Categories c ON e.CategoryId = c.Id
                    WHERE e.Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            expense = new Expense
                            {
                                Id = Convert.ToInt32(rdr["Id"]),
                                Amount = Convert.ToDecimal(rdr["Amount"]),
                                Description = rdr["Description"] as string,
                                ExpenseDate = Convert.ToDateTime(rdr["ExpenseDate"]),
                                CategoryId = Convert.ToInt32(rdr["CategoryId"]),
                                Category = new Category { Id = Convert.ToInt32(rdr["CategoryId"]), Name = rdr["CategoryName"].ToString() }
                            };
                        }
                    }
                }
            }
            return expense;
        }

        public void UpdateExpense(Expense expense)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Expenses SET Amount = @Amount, Description = @Description, ExpenseDate = @ExpenseDate, CategoryId = @CategoryId WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Amount", expense.Amount);
                    cmd.Parameters.AddWithValue("@Description", (object)expense.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExpenseDate", expense.ExpenseDate);
                    cmd.Parameters.AddWithValue("@CategoryId", expense.CategoryId);
                    cmd.Parameters.AddWithValue("@Id", expense.Id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteExpense(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Expenses WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public decimal GetTotalIncome()
        {
            decimal totalIncome = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT ISNULL(SUM(Amount), 0) FROM Incomes"; 
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                 
                    totalIncome = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            return totalIncome;
        }

        public decimal GetTotalExpenses()
        {
            decimal totalExpenses = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT ISNULL(SUM(Amount), 0) FROM Expenses";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    totalExpenses = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            return totalExpenses;
        }
    }
}