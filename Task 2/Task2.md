1. Open SQL Server Management Studio (or an equivalent SQL Management Software)
2. Connect to Northwind DB (Same DB as you used in Geek Week)
3. Write a query that shows each employee’s first name and the number of orders they have placed. Group the results by employee’s first name.

Please add your SQL query below AND add explanation of the query: 

```
[Insert Code HERE]
SELECT e.FirstName, COUNT(o.OrderID) AS NumberOfOrders
   FROM Employees AS e
   LEFT JOIN Orders AS o ON e.EmployeeID = o.EmployeeID
   GROUP BY e.FirstName
   ORDER BY NumberOfOrders DESC
```
```
[Insert Explanation Here] I joined the Employees and Orders tables on EmployeeID, grouped by first name, and counted the orders for each employee.
```

4. For submission commit and push this .md file to Github
