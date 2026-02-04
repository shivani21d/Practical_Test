-- ============================================================
-- 5.1 Top 5 customers by total order value
-- ============================================================
-- Assumes: Orders (CustomerId, TotalAmount), Customers (CustomerId, Name or similar)

SELECT TOP 5
    c.CustomerId,
    c.Name AS CustomerName,
    SUM(o.TotalAmount) AS TotalOrderValue
FROM Orders o
INNER JOIN Customers c ON c.CustomerId = o.CustomerId
GROUP BY c.CustomerId, c.Name
ORDER BY TotalOrderValue DESC;


-- ============================================================
-- 5.2 Total amount by customer in current month, highest first
-- ============================================================
-- Assumes: Orders (OrderId, CustomerId, OrderDate, TotalAmount)

SELECT
    CustomerId,
    SUM(TotalAmount) AS TotalSpentThisMonth
FROM Orders
WHERE YEAR(OrderDate) = YEAR(GETDATE())
  AND MONTH(OrderDate) = MONTH(GETDATE())
GROUP BY CustomerId
ORDER BY TotalSpentThisMonth DESC;


-- ============================================================
-- 5.3 Latest order per customer (all columns from Orders)
-- ============================================================
-- Assumes: Orders (OrderId, CustomerId, OrderDate, TotalAmount)

WITH RankedOrders AS (
    SELECT
        OrderId,
        CustomerId,
        OrderDate,
        TotalAmount,
        ROW_NUMBER() OVER (PARTITION BY CustomerId ORDER BY OrderDate DESC) AS rn
    FROM Orders
)
SELECT OrderId, CustomerId, OrderDate, TotalAmount
FROM RankedOrders
WHERE rn = 1;


-- ============================================================
-- 5.4 Optimization: Original query and suggested improvements
-- ============================================================
-- Original:
-- SELECT *
-- FROM Orders
-- WHERE CustomerId IN (SELECT CustomerId FROM Customers WHERE Country = 'USA');

-- Issues:
-- 1. SELECT * pulls all columns; can prevent index-only scans and increases I/O.
-- 2. IN (subquery) may not be optimized the same as a JOIN on some engines.
-- 3. No index hints or filters on Orders (e.g. date range) to reduce rows early.

-- Improved query (explicit columns, JOIN, optional date filter):

SELECT
    o.OrderId,
    o.CustomerId,
    o.OrderDate,
    o.TotalAmount
    -- list only columns you need
FROM Orders o
INNER JOIN Customers c ON c.CustomerId = o.CustomerId AND c.Country = 'USA';
-- Optional: AND o.OrderDate >= @StartDate AND o.OrderDate < @EndDate

-- Additional recommendations:
-- - Create index on Customers(Country, CustomerId).
-- - Create index on Orders(CustomerId) and optionally (CustomerId, OrderDate).
-- - Replace SELECT * with explicit column list in all queries.
