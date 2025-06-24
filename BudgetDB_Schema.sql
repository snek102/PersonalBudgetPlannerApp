USE BudgetDB;
GO

-- Create Categories Table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Id will auto-increment starting from 1
    Name NVARCHAR(100) NOT NULL UNIQUE -- Category names must be unique and cannot be empty
);

-- Insert some default categories
INSERT INTO Categories (Name) VALUES
('Food'),
('Transportation'),
('Rent'),
('Utilities'),
('Entertainment'),
('Salary'),
('Freelance'),
('Investment');


-- Create Incomes Table
CREATE TABLE Incomes (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Id will auto-increment
    Amount DECIMAL(18, 2) NOT NULL, -- Stores monetary amounts with 18 total digits, 2 after decimal
    Description NVARCHAR(255), -- Optional description for the income
    IncomeDate DATETIME NOT NULL DEFAULT GETDATE(), -- Date of income, defaults to current date/time
    CategoryId INT, -- Foreign key linking to Categories table (optional for income)
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) -- Establishes relationship
);

-- Create Expenses Table
CREATE TABLE Expenses (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Id will auto-increment
    Amount DECIMAL(18, 2) NOT NULL, -- Stores monetary amounts
    Description NVARCHAR(255), -- Optional description for the expense
    ExpenseDate DATETIME NOT NULL DEFAULT GETDATE(), -- Date of expense, defaults to current date/time
    CategoryId INT NOT NULL, -- Foreign key linking to Categories table (required for expense)
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) -- Establishes relationship
);