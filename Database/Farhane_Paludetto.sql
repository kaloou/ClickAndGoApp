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
    description VARCHAR(500),
    imagePath   VARCHAR(255),
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
    description VARCHAR(500), 
    imagePath VARCHAR(255) NULL
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
-- Données de test
-- =============================================
    
-- =============================================
-- STORES
-- =============================================
INSERT INTO Store (name, address) VALUES
                                      ('Click & Go Jumet', 'Chaussée de Bruxelles 167, 6040 Jumet'),
                                      ('Click & Go Gosselies', 'Rue de la Motte 4, 6041 Gosselies'),
                                      ('Click & Go Marcinelle',     'Rue de Philippeville 256, 6001 Marcinelle'),
                                      ('Click & Go Gilly',     'Rue de Ransart 80, 6060 Gilly');
GO

-- =============================================
-- CATEGORIES
-- =============================================
INSERT INTO Category (name) VALUES
                                ('Fruits et légumes'),      -- 1
                                ('Produits laitiers'),      -- 2
                                ('Viandes et volailles'),   -- 3
                                ('Poissons'),               -- 4
                                ('Boulangerie'),            -- 5
                                ('Boissons'),               -- 6
                                ('Épicerie sèche');         -- 7
GO

-- =============================================
-- PRODUCTS (46 en tout, ~6-7 par catégorie)
-- =============================================

-- Fruits et légumes (cat 1) → produits 1-7
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Tomate cerise',    1.99, 1, 'Tomates cerises sucrées, 500g',       NULL),
                                                                          ('Pomme Gala',       0.99, 1, 'Pomme croquante et sucrée',            NULL),
                                                                          ('Banane',           0.79, 1, 'Banane mûre à point',                  NULL),
                                                                          ('Courgette',        0.89, 1, 'Courgette verte fraîche',              NULL),
                                                                          ('Carotte',          0.59, 1, 'Carotte de plein champ, 1kg',          NULL),
                                                                          ('Salade iceberg',   1.29, 1, 'Laitue iceberg croquante',             NULL),
                                                                          ('Poivron rouge',    1.49, 1, 'Poivron rouge charnu',                 NULL);

-- Produits laitiers (cat 2) → produits 8-13
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Lait demi-écrémé', 1.49, 2, 'Lait demi-écrémé UHT, 1L',            'product/milk.png'),
                                                                          ('Yaourt nature',    0.49, 2, 'Yaourt au lait entier, 125g',          NULL),
                                                                          ('Beurre doux',      2.99, 2, 'Beurre doux 82% MG, 250g',            NULL),
                                                                          ('Fromage Gouda',    3.49, 2, 'Gouda affiné, tranche 200g',           NULL),
                                                                          ('Crème fraîche',    1.79, 2, 'Crème fraîche épaisse 40%, 20cl',      NULL),
                                                                          ('Emmental râpé',    2.29, 2, 'Emmental râpé, 150g',                  NULL);

-- Viandes et volailles (cat 3) → produits 14-20
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Blanc de poulet',   6.99, 3, 'Filets de poulet fermier, 500g',      NULL),
                                                                          ('Steak haché',       5.49, 3, 'Boeuf haché 15% MG, 2x150g',         NULL),
                                                                          ('Côte de porc',      4.99, 3, 'Côte de porc à griller',              NULL),
                                                                          ('Filet de boeuf',   14.99, 3, 'Filet de boeuf, pièce 200g',         NULL),
                                                                          ('Escalope de dinde', 5.99, 3, 'Escalope de dinde fine, 300g',        NULL),
                                                                          ('Merguez',           4.29, 3, 'Merguez agneau-boeuf, 400g',          NULL),
                                                                          ('Lardons fumés',     2.49, 3, 'Lardons fumés, 200g',                 NULL);

-- Poissons (cat 4) → produits 21-26
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Saumon frais',     9.99, 4, 'Saumon Atlantique, pavé 200g',         NULL),
                                                                          ('Thon en boîte',    1.99, 4, 'Thon au naturel, boîte 160g',          NULL),
                                                                          ('Crevettes roses',  5.99, 4, 'Crevettes cuites décortiquées, 200g',  NULL),
                                                                          ('Cabillaud',        8.49, 4, 'Filet de cabillaud frais, 300g',       NULL),
                                                                          ('Sardines',         1.49, 4, 'Sardines à l''huile, boîte 125g',      NULL),
                                                                          ('Moules',           3.99, 4, 'Moules de bouchot, 1kg',               NULL);

-- Boulangerie (cat 5) → produits 27-32
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Pain de campagne',  2.49, 5, 'Pain au levain à la croûte dorée',    NULL),
                                                                          ('Croissant',         0.89, 5, 'Croissant au beurre pur',             NULL),
                                                                          ('Baguette tradition',1.10, 5, 'Baguette tradition artisanale',       NULL),
                                                                          ('Pain complet',      2.19, 5, 'Pain complet aux graines, 500g',      NULL),
                                                                          ('Brioche',           3.49, 5, 'Brioche moelleuse, 400g',             NULL),
                                                                          ('Muffin chocolat',   1.29, 5, 'Muffin aux pépites de chocolat',      NULL);

-- Boissons (cat 6) → produits 33-39
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Eau minérale',     0.59, 6, 'Eau minérale naturelle, 1.5L',         NULL),
                                                                          ('Jus d''orange',    1.99, 6, 'Jus d''orange pressé, 1L',             NULL),
                                                                          ('Limonade',         1.49, 6, 'Limonade pétillante, 1.5L',            NULL),
                                                                          ('Café moulu',       4.99, 6, 'Café arabica moulu, 250g',             NULL),
                                                                          ('Thé vert',         2.49, 6, 'Thé vert sencha, 20 sachets',          NULL),
                                                                          ('Lait d''amande',   2.29, 6, 'Boisson végétale amande, 1L',          NULL),
                                                                          ('Coca-Cola',        1.79, 6, 'Coca-Cola, bouteille 1.5L',            NULL);

-- Épicerie sèche (cat 7) → produits 40-46
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Pâtes tagliatelles',1.49, 7, 'Tagliatelles aux oeufs, 500g',        NULL),
                                                                          ('Riz long grain',    1.29, 7, 'Riz blanc long grain, 1kg',           NULL),
                                                                          ('Lentilles vertes',  1.19, 7, 'Lentilles vertes du Puy, 500g',       NULL),
                                                                          ('Farine de blé',     0.89, 7, 'Farine T55, 1kg',                     NULL),
                                                                          ('Sucre en poudre',   0.99, 7, 'Sucre blanc en poudre, 1kg',          NULL),
                                                                          ('Huile d''olive',    4.99, 7, 'Huile d''olive vierge extra, 75cl',   NULL),
                                                                          ('Sauce tomate',      1.79, 7, 'Sauce tomate basilic, bocal 400g',    NULL);
GO

-- =============================================
-- RECIPES (6)
-- =============================================
INSERT INTO Recipes (name, description) VALUES
                                            ('Pâtes bolognaise',       'Un classique italien réconfortant, facile et rapide à préparer.'),
                                            ('Curry de poulet au riz', 'Un curry parfumé et doux servi sur un lit de riz.'),
                                            ('Salade César',           'La célèbre salade croustillante et crémeuse.'),
                                            ('Smoothie banane-amande', 'Un smoothie énergisant pour bien commencer la journée.'),
                                            ('Poêlée de légumes',      'Un mélange coloré de légumes sautés à l''huile d''olive.'),
                                            ('Saumon à la crème',      'Pavé de saumon nappé d''une sauce crémeuse sur lit de riz.');
GO

-- =============================================
-- RECIPES INGREDIENTS
-- =============================================
-- Pâtes bolognaise (1) : tagliatelles(40) + steak haché(15) + sauce tomate(46)
INSERT INTO RecipesIngredients (recipeId, productId, quantity) VALUES
                                                                   (1, 40, 2), (1, 15, 1), (1, 46, 2);

-- Curry de poulet au riz (2) : blanc poulet(14) + riz(41) + crème fraîche(12)
INSERT INTO RecipesIngredients (recipeId, productId, quantity) VALUES
                                                                   (2, 14, 1), (2, 41, 2), (2, 12, 1);

-- Salade César (3) : salade(6) + blanc poulet(14) + emmental(13)
INSERT INTO RecipesIngredients (recipeId, productId, quantity) VALUES
                                                                   (3, 6, 1), (3, 14, 1), (3, 13, 1);

-- Smoothie banane-amande (4) : banane(3) + yaourt(9) + lait d'amande(38)
INSERT INTO RecipesIngredients (recipeId, productId, quantity) VALUES
                                                                   (4, 3, 2), (4, 9, 1), (4, 38, 1);

-- Poêlée de légumes (5) : courgette(4) + poivron(7) + carotte(5) + huile d'olive(45)
INSERT INTO RecipesIngredients (recipeId, productId, quantity) VALUES
                                                                   (5, 4, 2), (5, 7, 1), (5, 5, 2), (5, 45, 1);

-- Saumon à la crème (6) : saumon(21) + crème fraîche(12) + riz(41)
INSERT INTO RecipesIngredients (recipeId, productId, quantity) VALUES
                                                                   (6, 21, 1), (6, 12, 1), (6, 41, 2);
GO

-- =============================================
-- USERS : 6 employés + 2 customers
-- =============================================
INSERT INTO [User] (firstName, lastName, email, password) VALUES
                                                              ('Lucas',   'Dupont',   'lucas.picker@clickgo.com',    'password123'),  -- 1 OrderPicker store 1
                                                              ('Emma',    'Bernard',  'emma.picker@clickgo.com',     'password123'),  -- 2 OrderPicker store 1
                                                              ('Thomas',  'Moreau',   'thomas.picker@clickgo.com',   'password123'),  -- 3 OrderPicker store 2
                                                              ('Léa',     'Simon',    'lea.cashier@clickgo.com',     'password123'),  -- 4 Cashier store 1
                                                              ('Hugo',    'Laurent',  'hugo.cashier@clickgo.com',    'password123'),  -- 5 Cashier store 1
                                                              ('Camille', 'Michel',   'camille.cashier@clickgo.com', 'password123'),  -- 6 Cashier store 2
                                                              ('Alice',   'Martin',   'alice@test.com',              'password123'),  -- 7 Customer
                                                              ('Bob',     'Renard',   'bob@test.com',                'password123');  -- 8 Customer
GO

INSERT INTO Employee (userId, storeId) VALUES
                                           (1, 1), (2, 1), (3, 2),
                                           (4, 1), (5, 1), (6, 2);

INSERT INTO OrderPicker (userId) VALUES (1), (2), (3);
INSERT INTO Cashier     (userId) VALUES (4), (5), (6);

INSERT INTO Customer (userId, loyaltyPoints, phoneNumber, address) VALUES
                                                                       (7, 0, '0470111222', 'Rue des Lilas 5, 1000 Bruxelles'),
                                                                       (8, 0, '0470333444', 'Avenue du Parc 12, 1050 Bruxelles');
GO

-- =============================================
-- TIME SLOTS + ORDERS (dynamiques)
-- =============================================
DECLARE @today    DATETIME = CAST(CAST(GETDATE() AS DATE) AS DATETIME);
DECLARE @tomorrow DATETIME = DATEADD(day, 1, @today);

-- Créneaux aujourd'hui store 1 (pour le cashier)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
                                                       (DATEADD(hour,  9, @today), DATEADD(hour, 10, @today), 1),  -- 1
                                                       (DATEADD(hour, 10, @today), DATEADD(hour, 11, @today), 1),  -- 2
                                                       (DATEADD(hour, 11, @today), DATEADD(hour, 12, @today), 1),  -- 3
                                                       (DATEADD(hour, 14, @today), DATEADD(hour, 15, @today), 1),  -- 4
                                                       (DATEADD(hour, 15, @today), DATEADD(hour, 16, @today), 1);  -- 5

-- Créneaux demain store 1 (pour l'order picker)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
                                                       (DATEADD(hour,  9, @tomorrow), DATEADD(hour, 10, @tomorrow), 1),  -- 6
                                                       (DATEADD(hour, 10, @tomorrow), DATEADD(hour, 11, @tomorrow), 1),  -- 7
                                                       (DATEADD(hour, 11, @tomorrow), DATEADD(hour, 12, @tomorrow), 1),  -- 8
                                                       (DATEADD(hour, 14, @tomorrow), DATEADD(hour, 15, @tomorrow), 1),  -- 9
                                                       (DATEADD(hour, 15, @tomorrow), DATEADD(hour, 16, @tomorrow), 1);  -- 10

-- Commandes d'aujourd'hui (visible par le cashier)
INSERT INTO [Order] (orderDate, status, numberOfBoxes, returnedBoxes, pickupDate, paymentStatus, customerId, timeSlotId) VALUES
                                                                                                                             (GETDATE(), 'Pending', 2, 0, DATEADD(hour,  9, @today), 'AwaitingPayment', 7, 1),
                                                                                                                             (GETDATE(), 'Ready',   3, 0, DATEADD(hour, 10, @today), 'AwaitingPayment', 8, 2),
                                                                                                                             (GETDATE(), 'Pending', 1, 0, DATEADD(hour, 11, @today), 'AwaitingPayment', 7, 3),
                                                                                                                             (GETDATE(), 'Ready',   2, 0, DATEADD(hour, 14, @today), 'AwaitingPayment', 8, 4);

-- Commandes de demain (visible par l'order picker)
INSERT INTO [Order] (orderDate, status, numberOfBoxes, returnedBoxes, pickupDate, paymentStatus, customerId, timeSlotId) VALUES
                                                                                                                             (GETDATE(), 'Pending', 0, 0, DATEADD(hour,  9, @tomorrow), 'AwaitingPayment', 7, 6),
                                                                                                                             (GETDATE(), 'Pending', 0, 0, DATEADD(hour, 10, @tomorrow), 'AwaitingPayment', 8, 7),
                                                                                                                             (GETDATE(), 'Ready',   0, 0, DATEADD(hour, 11, @tomorrow), 'AwaitingPayment', 7, 8);

-- Produits dans les commandes
INSERT INTO OrderLine (orderId, productId, quantity) VALUES
                                                         (1,  1, 3), (1,  8, 2), (1, 40, 1),
                                                         (2, 14, 1), (2, 41, 2), (2, 46, 2),
                                                         (3, 21, 2), (3, 12, 1), (3, 33, 3),
                                                         (4,  3, 4), (4,  9, 3), (4, 38, 1),
                                                         (5,  1, 2), (5,  4, 1), (5, 45, 1),
                                                         (6, 40, 2), (6, 15, 1), (6, 46, 1),
                                                         (7,  6, 1), (7, 14, 1), (7, 13, 1);
GO

