CREATE DATABASE ShopDB;
GO

USE ShopDB;
GO

-- Customer
CREATE TABLE Customer (
    ID INT PRIMARY KEY,
    IsActive BIT NOT NULL
);

-- Category
CREATE TABLE Category (
    ID INT PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL
);

-- Product
CREATE TABLE Product (
    ID INT PRIMARY KEY Identity(1,1),
    Name NVARCHAR(100) NOT NULL,
    Price FLOAT NOT NULL,
    Stock INT NOT NULL,
    CategoryID INT FOREIGN KEY REFERENCES Category(ID),
    Atributes NVARCHAR(50), --culori
    Size NVARCHAR(50),
    Description NVARCHAR(255),
    FileUrl NVARCHAR(255),
    IsActive BIT NOT NULL
);

-- Cart
CREATE TABLE Cart (
    ID INT PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customer(ID),
    CreatedAt DATETIME NOT NULL,
    IsActive BIT NOT NULL
);

-- CartItem
CREATE TABLE CartItem (
    ID INT PRIMARY KEY,
    CartID INT FOREIGN KEY REFERENCES Cart(ID),
    ProductID INT FOREIGN KEY REFERENCES Product(ID),
    Quantity INT NOT NULL,
    IsActive BIT NOT NULL
);

-- Order
CREATE TABLE [Order] (
    ID INT PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customer(ID),
    OrderDate DATETIME NOT NULL,
    TotalAmount FLOAT NOT NULL,
    IsActive BIT NOT NULL
);

-- OrderDetail
CREATE TABLE OrderDetail (
    ID INT PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES [Order](ID),
    ProductID INT FOREIGN KEY REFERENCES Product(ID),
    Quantity INT NOT NULL,
    Price FLOAT NOT NULL,
    IsActive BIT NOT NULL
);

-- Wishlist
CREATE TABLE Wishlist (
    ID INT PRIMARY KEY,
    ProductID INT FOREIGN KEY REFERENCES Product(ID),
    CustomerID INT FOREIGN KEY REFERENCES Customer(ID),
    IsActive BIT NOT NULL
);
