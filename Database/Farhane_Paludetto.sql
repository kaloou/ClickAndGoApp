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
                                                                          ('Tomate cerise',    1.99, 1, 'Tomates cerises sucrées, 500g',       '/images/products/cherry_tomatoes.jpg'),
                                                                          ('Pomme Gala',       0.99, 1, 'Pomme croquante et sucrée',            '/images/products/gala_apple.jpg'),
                                                                          ('Banane',           0.79, 1, 'Banane mûre à point',                  '/images/products/banana.jpg'),
                                                                          ('Courgette',        0.89, 1, 'Courgette verte fraîche',              '/images/products/zucchini.jpg'),
                                                                          ('Carotte',          0.59, 1, 'Carotte de plein champ, 1kg',          '/images/products/carrot.jpg'),
                                                                          ('Salade iceberg',   1.29, 1, 'Laitue iceberg croquante',             '/images/products/iceberg_letuce.jpg'),
                                                                          ('Poivron rouge',    1.49, 1, 'Poivron rouge charnu',                 '/images/products/red_pepper.jpg');

-- Produits laitiers (cat 2) → produits 8-13
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Lait demi-écrémé', 1.49, 2, 'Lait demi-écrémé UHT, 1L',            '/images/products/semi-skimmed_milk.jpg'),
                                                                          ('Yaourt nature',    0.49, 2, 'Yaourt au lait entier, 125g',          '/images/products/natural_yogurt.jpg'),
                                                                          ('Beurre doux',      2.99, 2, 'Beurre doux 82% MG, 250g',            '/images/products/soft_butter.jpeg'),
                                                                          ('Fromage Gouda',    3.49, 2, 'Gouda affiné, tranche 200g',           '/images/products/gouda_cheese.jpg'),
                                                                          ('Crème fraîche',    1.79, 2, 'Crème fraîche épaisse 40%, 20cl',      '/images/products/sour_cream.jpg'),
                                                                          ('Emmental râpé',    2.29, 2, 'Emmental râpé, 150g',                  '/images/products/emmental_snuff.jpg');

-- Viandes et volailles (cat 3) → produits 14-20
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Blanc de poulet',   6.99, 3, 'Filets de poulet fermier, 500g',      '/images/products/chicken_breast.jpg'),
                                                                          ('Steak haché',       5.49, 3, 'Boeuf haché 15% MG, 2x150g',         '/images/products/chopped_steak.jpg'),
                                                                          ('Côte de porc',      4.99, 3, 'Côte de porc à griller',              '/images/products/pork_chop.jpg'),
                                                                          ('Filet de boeuf',   14.99, 3, 'Filet de boeuf, pièce 200g',         '/images/products/beef_fillet.jpg'),
                                                                          ('Escalope de dinde', 5.99, 3, 'Escalope de dinde fine, 300g',        '/images/products/turkey_escalope.jpg'),
                                                                          ('Merguez',           4.29, 3, 'Merguez agneau-boeuf, 400g',          '/images/products/merguez.jpg'),
                                                                          ('Lardons fumés',     2.49, 3, 'Lardons fumés, 200g',                 '/images/products/smoked_bacon.jpg');

-- Poissons (cat 4) → produits 21-26
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Saumon frais',     9.99, 4, 'Saumon Atlantique, pavé 200g',         '/images/products/fresh_salmon.jpg'),
                                                                          ('Thon en boîte',    1.99, 4, 'Thon au naturel, boîte 160g',          '/images/products/canned_tuna.jpg'),
                                                                          ('Crevettes roses',  5.99, 4, 'Crevettes cuites décortiquées, 200g',  '/images/products/pink_shrimp.jpeg'),
                                                                          ('Cabillaud',        8.49, 4, 'Filet de cabillaud frais, 300g',       '/images/products/cod.jpg'),
                                                                          ('Sardines',         1.49, 4, 'Sardines à l''huile, boîte 125g',      '/images/products/sardinnes.jpg'),
                                                                          ('Moules',           3.99, 4, 'Moules de bouchot, 1kg',               '/images/products/mussels.jpg');

-- Boulangerie (cat 5) → produits 27-32
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Pain de campagne',  2.49, 5, 'Pain au levain à la croûte dorée',    '/images/products/country_bread.jpg'),
                                                                          ('Croissant',         0.89, 5, 'Croissant au beurre pur',             '/images/products/croissant.jpg'),
                                                                          ('Baguette tradition',1.10, 5, 'Baguette tradition artisanale',       '/images/products/traditional_baguette.jpg'),
                                                                          ('Pain complet',      2.19, 5, 'Pain complet aux graines, 500g',      '/images/products/wholemeal_bread.jpg'),
                                                                          ('Brioche',           3.49, 5, 'Brioche moelleuse, 400g',             '/images/products/bun.jpg'),
                                                                          ('Muffin chocolat',   1.29, 5, 'Muffin aux pépites de chocolat',      '/images/products/chocolate_muffin.jpg');

-- Boissons (cat 6) → produits 33-39
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Eau minérale',     0.59, 6, 'Eau minérale naturelle, 1.5L',         '/images/products/mineral_water.jpeg'),
                                                                          ('Jus d''orange',    1.99, 6, 'Jus d''orange pressé, 1L',             '/images/products/orange_juice.jpg'),
                                                                          ('Limonade',         1.49, 6, 'Limonade pétillante, 1.5L',            '/images/products/lemonade.jpg'),
                                                                          ('Café moulu',       4.99, 6, 'Café arabica moulu, 250g',             '/images/products/ground_coffee.jpg'),
                                                                          ('Thé vert',         2.49, 6, 'Thé vert sencha, 20 sachets',          '/images/products/green_tea.jpg'),
                                                                          ('Lait d''amande',   2.29, 6, 'Boisson végétale amande, 1L',          '/images/products/almond_milk.jpg'),
                                                                          ('Coca-Cola',        1.79, 6, 'Coca-Cola, bouteille 1.5L',            '/images/products/coca_cola.jpg');

-- Épicerie sèche (cat 7) → produits 40-46
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
                                                                          ('Pâtes tagliatelles',1.49, 7, 'Tagliatelles aux oeufs, 500g',        '/images/products/tagliatate_pasta.jpeg'),
                                                                          ('Riz long grain',    1.29, 7, 'Riz blanc long grain, 1kg',           '/images/products/long_grain_rice.jpg'),
                                                                          ('Lentilles vertes',  1.19, 7, 'Lentilles vertes du Puy, 500g',       '/images/products/green_lentils.jpg'),
                                                                          ('Farine de blé',     0.89, 7, 'Farine T55, 1kg',                     '/images/products/wheat_flour.jpg'),
                                                                          ('Sucre en poudre',   0.99, 7, 'Sucre blanc en poudre, 1kg',          '/images/products/powdered_sugar.jpg'),
                                                                          ('Huile d''olive',    4.99, 7, 'Huile d''olive vierge extra, 75cl',   '/images/products/olive_oil.jpg'),
                                                                          ('Sauce tomate',      1.79, 7, 'Sauce tomate basilic, bocal 400g',    '/images/products/tomato_sauce.jpg');
GO

-- =============================================
-- RECIPES (6)
-- =============================================
INSERT INTO Recipes (name, description, imagePath) VALUES
                                            ('Pâtes bolognaise',       'Un classique italien réconfortant, facile et rapide à préparer.',  'recipes/pasta_bolognese.jpg'),
                                            ('Curry de poulet au riz', 'Un curry parfumé et doux servi sur un lit de riz.',                'recipes/chicken_curry_with_rice.jpg'),
                                            ('Salade César',           'La célèbre salade croustillante et crémeuse.',                    'recipes/caesar_salad.jpg'),
                                            ('Smoothie banane-amande', 'Un smoothie énergisant pour bien commencer la journée.',           'recipes/banana_almond_smoothie.webp'),
                                            ('Poêlée de légumes',      'Un mélange coloré de légumes sautés à l''huile d''olive.',         'recipes/vegetable_stir-fry.jpg'),
                                            ('Saumon à la crème',      'Pavé de saumon nappé d''une sauce crémeuse sur lit de riz.',       'recipes/creamed_salmon.jpg');
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

-- =============================================
-- PATCH : ajout des imagePath sur la BD existante
-- À exécuter si la BD existe déjà sans les images
-- =============================================

-- Ajouter la colonne imagePath aux tables si elle n'existe pas encore
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'imagePath')
    ALTER TABLE Product ADD imagePath VARCHAR(255) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Recipes' AND COLUMN_NAME = 'imagePath')
    ALTER TABLE Recipes ADD imagePath VARCHAR(255) NULL;
GO

-- Produits - Fruits et légumes (1-7)
UPDATE Product SET imagePath = '/images/products/cherry_tomatoes.jpg'     WHERE productId = 1;
UPDATE Product SET imagePath = '/images/products/gala_apple.jpg'          WHERE productId = 2;
UPDATE Product SET imagePath = '/images/products/banana.jpg'              WHERE productId = 3;
UPDATE Product SET imagePath = '/images/products/zucchini.jpg'            WHERE productId = 4;
UPDATE Product SET imagePath = '/images/products/carrot.jpg'              WHERE productId = 5;
UPDATE Product SET imagePath = '/images/products/iceberg_letuce.jpg'      WHERE productId = 6;
UPDATE Product SET imagePath = '/images/products/red_pepper.jpg'          WHERE productId = 7;

-- Produits laitiers (8-13)
UPDATE Product SET imagePath = '/images/products/semi-skimmed_milk.jpg'   WHERE productId = 8;
UPDATE Product SET imagePath = '/images/products/natural_yogurt.jpg'      WHERE productId = 9;
UPDATE Product SET imagePath = '/images/products/soft_butter.jpeg'        WHERE productId = 10;
UPDATE Product SET imagePath = '/images/products/gouda_cheese.jpg'        WHERE productId = 11;
UPDATE Product SET imagePath = '/images/products/sour_cream.jpg'          WHERE productId = 12;
UPDATE Product SET imagePath = '/images/products/emmental_snuff.jpg'      WHERE productId = 13;

-- Viandes et volailles (14-20)
UPDATE Product SET imagePath = '/images/products/chicken_breast.jpg'      WHERE productId = 14;
UPDATE Product SET imagePath = '/images/products/chopped_steak.jpg'       WHERE productId = 15;
UPDATE Product SET imagePath = '/images/products/pork_chop.jpg'           WHERE productId = 16;
UPDATE Product SET imagePath = '/images/products/beef_fillet.jpg'         WHERE productId = 17;
UPDATE Product SET imagePath = '/images/products/turkey_escalope.jpg'     WHERE productId = 18;
UPDATE Product SET imagePath = '/images/products/merguez.jpg'             WHERE productId = 19;
UPDATE Product SET imagePath = '/images/products/smoked_bacon.jpg'        WHERE productId = 20;

-- Poissons (21-26)
UPDATE Product SET imagePath = '/images/products/fresh_salmon.jpg'        WHERE productId = 21;
UPDATE Product SET imagePath = '/images/products/canned_tuna.jpg'         WHERE productId = 22;
UPDATE Product SET imagePath = '/images/products/pink_shrimp.jpeg'        WHERE productId = 23;
UPDATE Product SET imagePath = '/images/products/cod.jpg'                 WHERE productId = 24;
UPDATE Product SET imagePath = '/images/products/sardinnes.jpg'           WHERE productId = 25;
UPDATE Product SET imagePath = '/images/products/mussels.jpg'             WHERE productId = 26;

-- Boulangerie (27-32)
UPDATE Product SET imagePath = '/images/products/country_bread.jpg'       WHERE productId = 27;
UPDATE Product SET imagePath = '/images/products/croissant.jpg'           WHERE productId = 28;
UPDATE Product SET imagePath = '/images/products/traditional_baguette.jpg' WHERE productId = 29;
UPDATE Product SET imagePath = '/images/products/wholemeal_bread.jpg'     WHERE productId = 30;
UPDATE Product SET imagePath = '/images/products/bun.jpg'                 WHERE productId = 31;
UPDATE Product SET imagePath = '/images/products/chocolate_muffin.jpg'    WHERE productId = 32;

-- Boissons (33-39)
UPDATE Product SET imagePath = '/images/products/mineral_water.webp'      WHERE productId = 33;
UPDATE Product SET imagePath = '/images/products/orange_juice.jpg'        WHERE productId = 34;
UPDATE Product SET imagePath = '/images/products/lemonade.jpg'            WHERE productId = 35;
UPDATE Product SET imagePath = '/images/products/ground_coffee.jpg'       WHERE productId = 36;
UPDATE Product SET imagePath = '/images/products/green_tea.jpg'           WHERE productId = 37;
UPDATE Product SET imagePath = '/images/products/almond_milk.jpg'         WHERE productId = 38;
UPDATE Product SET imagePath = '/images/products/coca_cola.jpg'           WHERE productId = 39;

-- Épicerie sèche (40-46)
UPDATE Product SET imagePath = '/images/products/tagliatate_pasta.jpeg'   WHERE productId = 40;
UPDATE Product SET imagePath = '/images/products/long_grain_rice.jpg'     WHERE productId = 41;
UPDATE Product SET imagePath = '/images/products/green_lentils.jpg'       WHERE productId = 42;
UPDATE Product SET imagePath = '/images/products/wheat_flour.jpg'         WHERE productId = 43;
UPDATE Product SET imagePath = '/images/products/powdered_sugar.jpg'      WHERE productId = 44;
UPDATE Product SET imagePath = '/images/products/olive_oil.jpg'           WHERE productId = 45;
UPDATE Product SET imagePath = '/images/products/tomato_sauce.jpg'        WHERE productId = 46;
GO

-- Recettes (1-6)
UPDATE Recipes SET imagePath = 'recipes/pasta_bolognese.jpg'          WHERE recipeId = 1;
UPDATE Recipes SET imagePath = 'recipes/chicken_curry_with_rice.jpg'  WHERE recipeId = 2;
UPDATE Recipes SET imagePath = 'recipes/caesar_salad.jpg'             WHERE recipeId = 3;
UPDATE Recipes SET imagePath = 'recipes/banana_almond_smoothie.webp'  WHERE recipeId = 4;
UPDATE Recipes SET imagePath = 'recipes/vegetable_stir-fry.jpg'       WHERE recipeId = 5;
UPDATE Recipes SET imagePath = 'recipes/creamed_salmon.jpg'           WHERE recipeId = 6;
GO

Update Store Set name = 'Click & Go Jumet',      address = 'Rue Hubert Bastin 7, 6040 Jumet'             where storeId = 1;
Update Store Set name = 'Click & Go Gosselies',  address = 'Rue Tahon 37, 6041 Gosselies'               where storeId = 2;
Update Store Set name = 'Click & Go Marcinelle', address = 'Rue du Grand Pont 16, 6001 Charleroi'       where storeId = 3;
Update Store Set name = 'Click & Go Gilly',      address = 'Chaussée Impériale 61, 6060 Gilly'          where storeId = 4;


