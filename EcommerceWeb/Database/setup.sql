/* =====================================================================
   EcommerceWeb - Database setup script
   Target: SQL Server (Azure SQL / AWS RDS / SQL Server / LocalDB)
   Run once against a fresh database.
   ===================================================================== */

IF DB_ID('EcommerceDB') IS NULL
    CREATE DATABASE EcommerceDB;
GO

USE EcommerceDB;
GO

/* ------------------------------------------------------------------ */
/* Tables                                                              */
/* ------------------------------------------------------------------ */

-- 1. Users
IF OBJECT_ID('dbo.Users', 'U') IS NULL
CREATE TABLE Users (
    UserId        INT PRIMARY KEY IDENTITY(1,1),
    Name          NVARCHAR(100) NOT NULL,
    Email         NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash  NVARCHAR(255) NOT NULL,
    Role          NVARCHAR(20)  NOT NULL CHECK (Role IN ('Buyer','Seller','Admin')),
    ProfileImage  NVARCHAR(255) DEFAULT '',
    CreatedAt     DATETIME DEFAULT GETDATE()
);
GO

-- 2. Products
IF OBJECT_ID('dbo.Products', 'U') IS NULL
CREATE TABLE Products (
    ProductId   INT PRIMARY KEY IDENTITY(1,1),
    SellerId    INT NOT NULL,
    Name        NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500),
    Price       DECIMAL(10,2) NOT NULL CHECK (Price > 0),
    Stock       INT NOT NULL CHECK (Stock >= 0),
    ImagePath   NVARCHAR(255),
    SoldCount   INT DEFAULT 0,
    ViewCount   INT DEFAULT 0,
    CreatedAt   DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SellerId) REFERENCES Users(UserId)
);
GO

-- 3. ProductImages
IF OBJECT_ID('dbo.ProductImages', 'U') IS NULL
CREATE TABLE ProductImages (
    ImageId   INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NOT NULL,
    ImagePath NVARCHAR(255) NOT NULL,
    SortOrder INT DEFAULT 0,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO

-- 4. Orders
IF OBJECT_ID('dbo.Orders', 'U') IS NULL
CREATE TABLE Orders (
    OrderId       INT PRIMARY KEY IDENTITY(1,1),
    BuyerId       INT NOT NULL,
    OrderDate     DATETIME DEFAULT GETDATE(),
    TotalAmount   DECIMAL(10,2) NOT NULL,
    Status        NVARCHAR(20) DEFAULT 'Pending',
    PaymentMethod NVARCHAR(50) DEFAULT 'Cash on Delivery',
    FOREIGN KEY (BuyerId) REFERENCES Users(UserId)
);
GO

-- 5. OrderDetails
IF OBJECT_ID('dbo.OrderDetails', 'U') IS NULL
CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
    OrderId       INT NOT NULL,
    ProductId     INT NOT NULL,
    Quantity      INT NOT NULL CHECK (Quantity > 0),
    Price         DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO

-- 6. ShippingAddress
IF OBJECT_ID('dbo.ShippingAddress', 'U') IS NULL
CREATE TABLE ShippingAddress (
    AddressId   INT PRIMARY KEY IDENTITY(1,1),
    OrderId     INT NOT NULL,
    FullName    NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    Country     NVARCHAR(50),
    Province    NVARCHAR(50),
    City        NVARCHAR(50),
    AddressLine NVARCHAR(255),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);
GO

-- 7. Cart
IF OBJECT_ID('dbo.Cart', 'U') IS NULL
CREATE TABLE Cart (
    CartId    INT PRIMARY KEY IDENTITY(1,1),
    UserId    INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity  INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO

/* ------------------------------------------------------------------ */
/* Stored procedures, functions & triggers                            */
/* ------------------------------------------------------------------ */

-- Get products by seller
IF OBJECT_ID('dbo.GetProductsBySeller', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetProductsBySeller;
GO
CREATE PROCEDURE GetProductsBySeller
    @SellerId INT
AS
BEGIN
    SELECT * FROM Products WHERE SellerId = @SellerId;
END
GO

-- Insert product
IF OBJECT_ID('dbo.InsertProduct', 'P') IS NOT NULL
    DROP PROCEDURE dbo.InsertProduct;
GO
CREATE PROCEDURE InsertProduct
    @SellerId INT,
    @Name VARCHAR(100),
    @ProductDescription VARCHAR(500),
    @Price DECIMAL(10,2),
    @Stock INT,
    @Img VARCHAR(255)
AS
BEGIN
    INSERT INTO Products (SellerId, Name, Description, Price, Stock, ImagePath)
    VALUES (@SellerId, @Name, @ProductDescription, @Price, @Stock, @Img);
END
GO

-- Get product by id (table-valued function)
IF OBJECT_ID('dbo.GetProductById', 'IF') IS NOT NULL
    DROP FUNCTION dbo.GetProductById;
GO
CREATE FUNCTION GetProductById (@Id INT)
RETURNS TABLE
AS
RETURN
(
    SELECT * FROM Products WHERE ProductId = @Id
);
GO

-- Prevent negative stock
IF OBJECT_ID('dbo.trg_CheckStock', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_CheckStock;
GO
CREATE TRIGGER trg_CheckStock
ON Products
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS (SELECT * FROM inserted WHERE Stock < 0)
    BEGIN
        RAISERROR('Stock cannot be negative', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO
