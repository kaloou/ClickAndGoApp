-- Création de la BDD
CREATE DATABASE ClickAndCollect_Farhane_Paludetto;
GO

USE ClickAndCollect_Farhane_Paludetto;
GO

-- =============================================
-- TABLE USER
-- =============================================
CREATE TABLE [User] (
                        userId      INT IDENTITY(1,1) PRIMARY KEY,
    firstName   VARCHAR(50)  NOT NULL,
    lastName    VARCHAR(50)  NOT NULL,
    email       VARCHAR(100) NOT NULL UNIQUE,
    password    VARCHAR(255) NOT NULL
    );
GO

-- =============================================
-- TABLE CUSTOMER
-- =============================================
CREATE TABLE Customer (
                          userId        INT PRIMARY KEY,
                          loyaltyPoints INT          DEFAULT 0,
                          phoneNumber   VARCHAR(20),
                          address       VARCHAR(255),
                          FOREIGN KEY (userId) REFERENCES [User](userId)
);
GO

-- =============================================
-- TABLE STORE
-- =============================================
CREATE TABLE Store (
                       storeId INT IDENTITY(1,1) PRIMARY KEY,
                       name    VARCHAR(100) NOT NULL,
                       address VARCHAR(255) NOT NULL
);
GO

-- =============================================
-- TABLE EMPLOYEE
-- =============================================
CREATE TABLE Employee (
                          userId  INT PRIMARY KEY,
                          storeId INT NOT NULL,
                          FOREIGN KEY (userId)  REFERENCES [User](userId),
                          FOREIGN KEY (storeId) REFERENCES Store(storeId)
);
GO

-- =============================================
-- TABLE ORDERPICKER
-- =============================================
CREATE TABLE OrderPicker (
                             userId INT PRIMARY KEY,
                             FOREIGN KEY (userId) REFERENCES Employee(userId)
);
GO

-- =============================================
-- TABLE CASHIER
-- =============================================
CREATE TABLE Cashier (
                         userId INT PRIMARY KEY,
                         FOREIGN KEY (userId) REFERENCES Employee(userId)
);
GO

-- =============================================
-- TABLE CATEGORY
-- =============================================
CREATE TABLE Category (
                          categoryId INT IDENTITY(1,1) PRIMARY KEY,
                          name       VARCHAR(100) NOT NULL
);
GO

-- =============================================
-- TABLE PRODUCT
-- =============================================
CREATE TABLE Product (
                         productId  INT IDENTITY(1,1) PRIMARY KEY,
                         name       VARCHAR(100)   NOT NULL,
                         price      DECIMAL(10, 2) NOT NULL,
                         categoryId INT            NOT NULL,
                         imagePath  VARCHAR(255),
                         FOREIGN KEY (categoryId) REFERENCES Category(categoryId)
);
GO

-- =============================================
-- TABLE TIMESLOT
-- =============================================
CREATE TABLE TimeSlot (
                          timeSlotId INT IDENTITY(1,1) PRIMARY KEY,
                          startTime  DATETIME NOT NULL,
                          endTime    DATETIME NOT NULL,
                          storeId    INT      NOT NULL,
                          FOREIGN KEY (storeId) REFERENCES Store(storeId)
);
GO

-- =============================================
-- TABLE ORDER
-- =============================================
CREATE TABLE [Order] (
                         orderId       INT IDENTITY(1,1) PRIMARY KEY,
    orderDate     DATETIME       NOT NULL DEFAULT GETDATE(),
    status        VARCHAR(20)    NOT NULL DEFAULT 'Pending',
    numberOfBoxes INT            NOT NULL DEFAULT 0,
    returnedBoxes INT            NOT NULL DEFAULT 0,
    pickupDate    DATETIME       NOT NULL,
    paymentStatus VARCHAR(20)    NOT NULL DEFAULT 'AwaitingPayment',
    customerId    INT            NOT NULL,
    timeSlotId    INT            NOT NULL,
    FOREIGN KEY (customerId)  REFERENCES Customer(userId),
    FOREIGN KEY (timeSlotId)  REFERENCES TimeSlot(timeSlotId)
    );
GO

-- =============================================
-- TABLE ORDER LINE
-- =============================================
CREATE TABLE OrderLine (
                           orderId   INT NOT NULL,
                           productId INT NOT NULL,
                           quantity  INT NOT NULL DEFAULT 1,
                           PRIMARY KEY (orderId, productId),
                           FOREIGN KEY (orderId)   REFERENCES [Order](orderId),
                           FOREIGN KEY (productId) REFERENCES Product(productId)
);
GO

-- =============================================
-- TABLE RECIPES
-- =============================================
CREATE TABLE Recipes (
                         recipeId    INT IDENTITY(1,1) PRIMARY KEY,
                         name        VARCHAR(100) NOT NULL,
                         description VARCHAR(500)
);
GO

-- =============================================
-- TABLE RECIPES INGREDIENTS
-- =============================================
CREATE TABLE RecipesIngredients (
                                    recipeId  INT NOT NULL,
                                    productId INT NOT NULL,
                                    quantity  INT NOT NULL DEFAULT 1,
                                    PRIMARY KEY (recipeId, productId),
                                    FOREIGN KEY (recipeId)  REFERENCES Recipes(recipeId),
                                    FOREIGN KEY (productId) REFERENCES Product(productId)
);
GO

-- =============================================
-- DONNEES DE TEST
-- =============================================

-- Stores
INSERT INTO Store (name, address) VALUES
('Store Bruxelles', 'Rue de la Loi 1, 1000 Bruxelles'),
('Store Charleroi', 'Rue du Centre 5, 6000 Charleroi');

-- Categories
INSERT INTO Category (name) VALUES
                                ('Fruits et légumes'),
                                ('Produits laitiers'),
                                ('Boucherie'),
                                ('Surgelés'),
                                ('Huiles et épices');

-- Products
INSERT INTO Product (name, price, categoryId) VALUES
                                                  ('Tomate', 1.99, 1),
                                                  ('Pomme', 0.99, 1),
                                                  ('Lait', 1.49, 2),
                                                  ('Fromage', 3.99, 2),
                                                  ('Poulet', 7.99, 3);

-- Users
INSERT INTO [User] (firstName, lastName, email, password) VALUES
    ('Jean', 'Dupont', 'jean@store.com', 'password123'),
    ('Marie', 'Martin', 'marie@store.com', 'password123'),
    ('Client', 'Test', 'client@test.com', 'password123');

-- Employees
INSERT INTO Employee (userId, storeId) VALUES (1, 1), (2, 1);
INSERT INTO OrderPicker (userId) VALUES (1);
INSERT INTO Cashier (userId) VALUES (2);

-- Customer
INSERT INTO Customer (userId, loyaltyPoints, phoneNumber, address)
VALUES (3, 0, '0470123456', 'Rue Test 1, 1000 Bruxelles');

-- TimeSlots
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
                                                       ('2026-05-08 09:00:00', '2026-05-08 10:00:00', 1),
                                                       ('2026-05-08 10:00:00', '2026-05-08 11:00:00', 1),
                                                       ('2026-05-08 11:00:00', '2026-05-08 12:00:00', 1);
GO