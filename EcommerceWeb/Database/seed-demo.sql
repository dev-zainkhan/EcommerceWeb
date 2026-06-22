/* =====================================================================
   EcommerceWeb - Optional demo data
   Requires a Seller account with UserId = 1 (register one first).
   Safe to re-run: clears existing products/images/cart first.
   Images reference local placeholders in wwwroot/img/products.
   ===================================================================== */
USE EcommerceDB;
GO

DELETE FROM ProductImages;
DELETE FROM Cart;
DELETE FROM Products;
GO

DECLARE @SellerId INT = 1;

INSERT INTO Products (SellerId, Name, Description, Price, Stock, ImagePath) VALUES
(@SellerId, 'Wireless Noise-Cancelling Headphones', 'Over-ear Bluetooth headphones with active noise cancellation and 30-hour battery life.', 18999, 25, '/img/products/headphones1.svg'),
(@SellerId, 'Smart Fitness Watch', 'Track heart rate, sleep and workouts with a bright AMOLED display and 7-day battery.', 12499, 40, '/img/products/watch1.svg'),
(@SellerId, 'Leather Laptop Backpack', 'Water-resistant backpack with padded 15.6" laptop sleeve and USB charging port.', 6499, 15, '/img/products/backpack1.svg'),
(@SellerId, 'RGB Mechanical Keyboard', 'Hot-swappable mechanical switches with per-key RGB and a compact 75% layout.', 8999, 30, '/img/products/keyboard1.svg'),
(@SellerId, '4K Action Camera', 'Waterproof 4K/60fps action camera with image stabilization and dual screens.', 23999, 12, '/img/products/camera1.svg'),
(@SellerId, 'Polarized Sunglasses', 'UV400 polarized lenses in a lightweight aluminium frame. Unisex design.', 3499, 50, '/img/products/sunglasses1.svg');
GO

/* Three gallery images per product, resolved by product name */
DECLARE @img TABLE (Slug NVARCHAR(50), Name NVARCHAR(150));
INSERT INTO @img VALUES
('headphones','Wireless Noise-Cancelling Headphones'),
('watch','Smart Fitness Watch'),
('backpack','Leather Laptop Backpack'),
('keyboard','RGB Mechanical Keyboard'),
('camera','4K Action Camera'),
('sunglasses','Polarized Sunglasses');

INSERT INTO ProductImages (ProductId, ImagePath, SortOrder)
SELECT p.ProductId,
       '/img/products/' + i.Slug + CAST(n.k AS NVARCHAR(2)) + '.svg',
       n.k - 1
FROM @img i
JOIN Products p ON p.Name = i.Name
CROSS JOIN (SELECT 1 AS k UNION SELECT 2 UNION SELECT 3) n;
GO
