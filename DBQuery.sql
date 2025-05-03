CREATE DATABASE ShopDB;
GO

USE ShopDB;
GO

DROP TABLE IF EXISTS OrderItem;
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS CartItem;
DROP TABLE IF EXISTS WishlistItem;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS Category;
DROP TABLE IF EXISTS Customer;

CREATE TABLE Customer (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Category (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Product (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Stock INT NOT NULL,
    CategoryID INT NOT NULL,
    Size NVARCHAR(50) NOT NULL DEFAULT 'N/A',
    Color NVARCHAR(50) NOT NULL DEFAULT 'N/A',
    Description NVARCHAR(255) NOT NULL DEFAULT '',
    PhotoURL NVARCHAR(255),
    CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryID) REFERENCES Category(ID) ON DELETE CASCADE
);

CREATE TABLE CartItem (
	ID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    CustomerID INT NOT NULL,
    CONSTRAINT FK_CartItem_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(ID) ON DELETE CASCADE,
    CONSTRAINT FK_CartItem_Product FOREIGN KEY (ProductID) REFERENCES Product(ID) ON DELETE CASCADE
);

CREATE TABLE WishlistItem (
	ID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    CustomerID INT NOT NULL,
    CONSTRAINT FK_Wishlist_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(ID) ON DELETE CASCADE,
    CONSTRAINT FK_Wishlist_Product FOREIGN KEY (ProductID) REFERENCES Product(ID) ON DELETE CASCADE
);

CREATE TABLE [Order] (
    ID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(ID) ON DELETE CASCADE
);

CREATE TABLE OrderItem (
    ID INT PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderID) REFERENCES [Order](ID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetail_Product FOREIGN KEY (ProductID) REFERENCES Product(ID) ON DELETE CASCADE
);

-- Insert mock data
INSERT INTO Customer (Name) VALUES ('Alice'), ('Bob'), ('Charlie');

INSERT INTO Category (Name) VALUES ('Electronics'), ('Clothing'), ('Books');

INSERT INTO Product (Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL) VALUES
('Laptop', 999.99, 10, 1, '15-inch', 'Gray', 'Powerful laptop', 'laptop.jpg'),
('T-Shirt', 19.99, 50, 2, 'M', 'Blue', 'Comfortable cotton shirt', 'shirt.jpg'),
('Book', 9.99, 100, 3, DEFAULT, DEFAULT, DEFAULT, 'book.jpg');

INSERT INTO CartItem (CustomerID, ProductID) VALUES
(1, 1),
(2, 2);

INSERT INTO WishlistItem (CustomerID, ProductID) VALUES
(1, 3),
(3, 1);

INSERT INTO [Order] (CustomerID, OrderDate) VALUES
(1, GETDATE()),
(2, GETDATE());

INSERT INTO OrderItem (ID, OrderID, ProductID, Quantity) VALUES
(1, 1, 1, 1),
(2, 2, 2, 2);