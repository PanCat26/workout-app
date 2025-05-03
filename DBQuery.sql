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

INSERT INTO Category (Name) VALUES 
('Electronics'), 
('Clothing'), 
('Books'), 
('Fitness'), 
('Home Appliances');

INSERT INTO Product (Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL) VALUES
-- Electronics
('Smartphone', 699.99, 25, 1, '6.1-inch', 'Black', 'Latest smartphone with high-resolution display', 'https://dlcdnwebimgs.asus.com/files/media/30106838-7820-415e-baac-f0971bfa65b3/v1/features/images/large/1x/kv/phone_left.png'),
('Wireless Earbuds', 129.99, 40, 1, DEFAULT, 'White', 'Noise-cancelling wireless earbuds', 'https://m.media-amazon.com/images/I/51pycg0MGxL.jpg'),

-- Clothing
('Jeans', 49.99, 30, 2, '32', 'Blue', 'Slim-fit denim jeans', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQh-D_7D9Pt3fXZh8Eyp933oGjWHzzYjnSbrQ&s'),
('Hoodie', 39.99, 20, 2, 'L', 'Black', 'Warm fleece hoodie', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQsy7eTIB4Y1lGbQEHKEs7pi09zz-2uxwxOEQ&s'),

-- Books
('Science Fiction Novel', 14.99, 60, 3, DEFAULT, DEFAULT, 'A thrilling space adventure', 'https://cdn.shopify.com/s/files/1/0194/2855/files/atomic-habits_600x600.jpg?v=1624825894'),
('Cookbook', 24.99, 45, 3, DEFAULT, DEFAULT, 'Delicious recipes for all skill levels', 'https://m.media-amazon.com/images/I/811go9afNjL._AC_UF1000,1000_QL80_.jpg'),

-- Fitness
('Yoga Mat', 29.99, 15, 4, 'Standard', 'Purple', 'Non-slip yoga mat', 'https://www.techfit.ro/image/cache/catalog/ANA/PVCPRINT1/PVCPRINT1_7-1500x1500.jpg'),
('Dumbbells', 59.99, 10, 4, '10kg', 'Black', 'Pair of 10kg dumbbells', 'https://www.profesionalfitness.ro/wp-content/uploads/2020/07/PALLADIUM-DUMBBELLS-1-%E2%80%93-10-KG.jpg'),

-- Home Appliances
('Air Fryer', 89.99, 8, 5, DEFAULT, 'Silver', 'Healthy cooking air fryer', 'https://m.media-amazon.com/images/I/71ZhnVxe-0L.jpg'),
('Blender', 34.99, 12, 5, DEFAULT, 'Red', 'Multi-speed kitchen blender', 'https://m.media-amazon.com/images/I/81C9vKqIIBL.jpg');

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