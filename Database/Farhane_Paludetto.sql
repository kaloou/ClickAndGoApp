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
    productId   INT IDENTITY(1,1) PRIMARY KEY,
    name        VARCHAR(100)   NOT NULL,
    price       DECIMAL(10, 2) NOT NULL,
    categoryId  INT            NOT NULL,
    imagePath   VARCHAR(255),
    description VARCHAR(500),
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
    pickupDate    DATETIME       NULL,
    paymentStatus VARCHAR(20)    NOT NULL DEFAULT 'AwaitingPayment',
    customerId    INT            NOT NULL,
    timeSlotId    INT            NULL,
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

-- =============================
-- Données de Test
-- =============================

INSERT INTO Store (name, address) VALUES
    ('Store Bruxelles', 'Rue de la Loi 1, 1000 Bruxelles'),
    ('Store Charleroi', 'Rue du Centre 5, 6000 Charleroi');

INSERT INTO Category (name) VALUES
    ('Fruits et légumes'),
    ('Produits laitiers'),
    ('Boucherie'),
    ('Surgelés'),
    ('Huiles et épices');

INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Tomate',  1.99, 1, 'Tomate de saison',                  NULL),
    ('Pomme',   0.99, 1, 'Pomme juteuse',                     NULL),
    ('Lait',    1.49, 2, 'Lait de vache',                     'product/milk.png'),
    ('Fromage', 3.99, 2, 'Fromage affiné en montagne',        NULL),
    ('Poulet',  7.99, 3, 'Poulet AH NAN MAIS VRM LA',         NULL);

INSERT INTO [User] (firstName, lastName, email, password) VALUES
    ('Jean',    'Dupont', 'jean@store.com',   'password123'),
    ('Marie',   'Martin', 'marie@store.com',  'password123'),
    ('Alice',   'Dupuis', 'alice@test.com',   'password123'),
    ('Bob',     'Leroy',  'bob@test.com',     'password123'),
    ('Charlie', 'Renard', 'charlie@test.com', 'password123');

INSERT INTO Employee (userId, storeId) VALUES (1, 1), (2, 1);
INSERT INTO OrderPicker (userId) VALUES (1);
INSERT INTO Cashier (userId) VALUES (2);

INSERT INTO Customer (userId, loyaltyPoints, phoneNumber, address) VALUES
    (3, 0, '0470111111', 'Rue A 1, 1000 Bruxelles'),
    (4, 0, '0470222222', 'Rue B 2, 1000 Bruxelles'),
    (5, 0, '0470333333', 'Rue C 3, 1000 Bruxelles');

DECLARE @tomorrow DATETIME = DATEADD(day, 1, CAST(CAST(GETDATE() AS DATE) AS DATETIME));

INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    (DATEADD(hour,  9, @tomorrow), DATEADD(hour, 10, @tomorrow), 1),
    (DATEADD(hour, 10, @tomorrow), DATEADD(hour, 11, @tomorrow), 1),
    (DATEADD(hour, 11, @tomorrow), DATEADD(hour, 12, @tomorrow), 1),
    (DATEADD(hour, 14, @tomorrow), DATEADD(hour, 15, @tomorrow), 1),
    (DATEADD(hour, 15, @tomorrow), DATEADD(hour, 16, @tomorrow), 1);

INSERT INTO [Order] (orderDate, status, numberOfBoxes, returnedBoxes, pickupDate, paymentStatus, customerId, timeSlotId) VALUES
    (GETDATE(), 'Pending', 0, 0, DATEADD(hour,  9, @tomorrow), 'AwaitingPayment', 3, 1),
    (GETDATE(), 'Pending', 0, 0, DATEADD(hour, 10, @tomorrow), 'AwaitingPayment', 4, 2),
    (GETDATE(), 'Ready',   0, 0, DATEADD(hour, 11, @tomorrow), 'AwaitingPayment', 5, 3),
    (GETDATE(), 'Pending', 0, 0, DATEADD(hour, 14, @tomorrow), 'AwaitingPayment', 3, 4),
    (GETDATE(), 'Ready',   0, 0, DATEADD(hour, 15, @tomorrow), 'AwaitingPayment', 4, 5),
    (GETDATE(), 'Pending', 0, 0, DATEADD(hour,  18, @tomorrow), 'AwaitingPayment', 5, 1);

INSERT INTO OrderLine (orderId, productId, quantity) VALUES
    (1, 1, 3), (1, 3, 2),
    (2, 2, 5), (2, 4, 1),
    (3, 5, 2), (3, 1, 4),
    (4, 2, 1), (4, 5, 3),
    (5, 3, 2), (5, 4, 2),
    (6, 1, 6), (6, 2, 3);
GO
