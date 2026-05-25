-- =============================================
-- Création de la Base de données : Farhane_Paludetto
-- =============================================

USE [master]
GO

DROP DATABASE [Farhane_Paludetto]
CREATE DATABASE [Farhane_Paludetto]
GO

USE [Farhane_Paludetto]
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
    imagePath   VARCHAR(255) NULL
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
    ('Click & Go Jumet',      'Rue Hubert Bastin 7, 6040 Jumet'),
    ('Click & Go Gosselies',  'Rue Tahon 37, 6041 Gosselies'),
    ('Click & Go Marcinelle', 'Rue du Grand Pont 16, 6001 Charleroi'),
    ('Click & Go Gilly',      'Chaussée Impériale 61, 6060 Gilly');
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
-- PRODUCTS (71 en tout, ~6-7 par catégorie)
-- =============================================

-- Fruits et légumes (cat 1) => produits 1-7
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Tomate cerise',    1.99, 1, 'Tomates cerises sucrées, 500g',       '/images/products/cherry_tomatoes.jpg'),
    ('Pomme Gala',       0.99, 1, 'Pomme croquante et sucrée',            '/images/products/gala_apple.jpg'),
    ('Banane',           0.79, 1, 'Banane mûre à point',                  '/images/products/banana.jpg'),
    ('Courgette',        0.89, 1, 'Courgette verte fraîche',              '/images/products/zucchini.jpg'),
    ('Carotte',          0.59, 1, 'Carotte de plein champ, 1kg',          '/images/products/carrot.jpg'),
    ('Salade iceberg',   1.29, 1, 'Laitue iceberg croquante',             '/images/products/iceberg_letuce.jpg'),
    ('Poivron rouge',    1.49, 1, 'Poivron rouge charnu',                 '/images/products/red_pepper.jpg');

-- Produits laitiers (cat 2) => produits 8-13
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Lait demi-écrémé', 1.49, 2, 'Lait demi-écrémé UHT, 1L',            '/images/products/semi-skimmed_milk.jpg'),
    ('Yaourt nature',    0.49, 2, 'Yaourt au lait entier, 125g',          '/images/products/natural_yogurt.jpg'),
    ('Beurre doux',      2.99, 2, 'Beurre doux 82% MG, 250g',            '/images/products/soft_butter.jpeg'),
    ('Fromage Gouda',    3.49, 2, 'Gouda affiné, tranche 200g',           '/images/products/gouda_cheese.jpg'),
    ('Crème fraîche',    1.79, 2, 'Crème fraîche épaisse 40%, 20cl',      '/images/products/sour_cream.jpg'),
    ('Emmental râpé',    2.29, 2, 'Emmental râpé, 150g',                  '/images/products/emmental_snuff.jpg');

-- Viandes et volailles (cat 3) => produits 14-20
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Blanc de poulet',   6.99, 3, 'Filets de poulet fermier, 500g',      '/images/products/chicken_breast.jpg'),
    ('Steak haché',       5.49, 3, 'Boeuf haché 15% MG, 2x150g',         '/images/products/chopped_steak.jpg'),
    ('Côte de porc',      4.99, 3, 'Côte de porc à griller',              '/images/products/pork_chop.jpg'),
    ('Filet de boeuf',   14.99, 3, 'Filet de boeuf, pièce 200g',         '/images/products/beef_fillet.jpg'),
    ('Escalope de dinde', 5.99, 3, 'Escalope de dinde fine, 300g',        '/images/products/turkey_escalope.jpg'),
    ('Merguez',           4.29, 3, 'Merguez agneau-boeuf, 400g',          '/images/products/merguez.jpg'),
    ('Lardons fumés',     2.49, 3, 'Lardons fumés, 200g',                 '/images/products/smoked_bacon.jpg');

-- Poissons (cat 4) => produits 21-26
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Saumon frais',     9.99, 4, 'Saumon Atlantique, pavé 200g',         '/images/products/fresh_salmon.jpg'),
    ('Thon en boîte',    1.99, 4, 'Thon au naturel, boîte 160g',          '/images/products/canned_tuna.jpg'),
    ('Crevettes roses',  5.99, 4, 'Crevettes cuites décortiquées, 200g',  '/images/products/pink_shrimp.jpeg'),
    ('Cabillaud',        8.49, 4, 'Filet de cabillaud frais, 300g',       '/images/products/cod.jpg'),
    ('Sardines',         1.49, 4, 'Sardines à l''huile, boîte 125g',      '/images/products/sardinnes.jpg'),
    ('Moules',           3.99, 4, 'Moules de bouchot, 1kg',               '/images/products/mussels.jpg');

-- Boulangerie (cat 5) => produits 27-32
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Pain de campagne',   2.49, 5, 'Pain au levain à la croûte dorée',   '/images/products/country_bread.jpg'),
    ('Croissant',          0.89, 5, 'Croissant au beurre pur',             '/images/products/croissant.jpg'),
    ('Baguette tradition', 1.10, 5, 'Baguette tradition artisanale',       '/images/products/traditional_baguette.jpg'),
    ('Pain complet',       2.19, 5, 'Pain complet aux graines, 500g',      '/images/products/wholemeal_bread.jpg'),
    ('Brioche',            3.49, 5, 'Brioche moelleuse, 400g',             '/images/products/bun.jpg'),
    ('Muffin chocolat',    1.29, 5, 'Muffin aux pépites de chocolat',      '/images/products/chocolate_muffin.jpg');

-- Boissons (cat 6) => produits 33-39
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Eau minérale',    0.59, 6, 'Eau minérale naturelle, 1.5L',          '/images/products/mineral_water.jpeg'),
    ('Jus d''orange',   1.99, 6, 'Jus d''orange pressé, 1L',              '/images/products/orange_juice.jpg'),
    ('Limonade',        1.49, 6, 'Limonade pétillante, 1.5L',             '/images/products/lemonade.jpg'),
    ('Café moulu',      4.99, 6, 'Café arabica moulu, 250g',              '/images/products/ground_coffee.jpg'),
    ('Thé vert',        2.49, 6, 'Thé vert sencha, 20 sachets',           '/images/products/green_tea.jpg'),
    ('Lait d''amande',  2.29, 6, 'Boisson végétale amande, 1L',           '/images/products/almond_milk.jpg'),
    ('Coca-Cola',       1.79, 6, 'Coca-Cola, bouteille 1.5L',             '/images/products/coca_cola.jpg');

-- Épicerie sèche (cat 7) => produits 40-46
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Pâtes tagliatelles', 1.49, 7, 'Tagliatelles aux oeufs, 500g',       '/images/products/tagliatate_pasta.jpeg'),
    ('Riz long grain',     1.29, 7, 'Riz blanc long grain, 1kg',          '/images/products/long_grain_rice.jpg'),
    ('Lentilles vertes',   1.19, 7, 'Lentilles vertes du Puy, 500g',      '/images/products/green_lentils.jpg'),
    ('Farine de blé',      0.89, 7, 'Farine T55, 1kg',                    '/images/products/wheat_flour.jpg'),
    ('Sucre en poudre',    0.99, 7, 'Sucre blanc en poudre, 1kg',         '/images/products/powdered_sugar.jpg'),
    ('Huile d''olive',     4.99, 7, 'Huile d''olive vierge extra, 75cl',  '/images/products/olive_oil.jpg'),
    ('Sauce tomate',       1.79, 7, 'Sauce tomate basilic, bocal 400g',   '/images/products/tomato_sauce.jpg');

-- Fruits et légumes (cat 1) => produits 47-50
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Fraises',          3.49, 1, 'Fraises de saison, barquette 250g',    '/images/products/strawberries.jpg'),
    ('Avocat',           1.29, 1, 'Avocat Hass bien mûr',                 '/images/products/avocado.jpg'),
    ('Épinards frais',   1.99, 1, 'Épinards jeunes pousses, sachet 200g', '/images/products/spinach.jpg'),
    ('Oignons jaunes',   1.49, 1, 'Oignons jaunes filet, 1kg',            '/images/products/yellow_onions.jpg');

-- Produits laitiers (cat 2) => produits 51-54
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Mozzarella',             1.79, 2, 'Mozzarella di bufala, 125g',          '/images/products/mozzarella.jpg'),
    ('Fromage blanc',          1.99, 2, 'Fromage blanc 0% MG, 500g',           '/images/products/fromage_blanc.jpg'),
    ('Comté râpé',             3.99, 2, 'Comté AOP râpé finement, 150g',       '/images/products/comte_cheese.jpg'),
    ('Crème dessert chocolat', 0.99, 2, 'Crème dessert au chocolat, 4×125g',   '/images/products/chocolate_cream_dessert.jpg');

-- Viandes et volailles (cat 3) => produits 55-57
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Saucisses de porc', 4.49, 3, 'Saucisses de porc grillades x6, 400g',    '/images/products/pork_sausages.jpg'),
    ('Cuisses de poulet', 5.99, 3, 'Cuisses de poulet fermier x4, 700g',       '/images/products/chicken_thighs.jpg'),
    ('Jambon blanc',      2.99, 3, 'Jambon blanc supérieur x4 tranches, 160g', '/images/products/cooked_ham.jpg');

-- Poissons (cat 4) => produits 58-61
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Truite fumée',   5.49, 4, 'Truite fumée tranchée, 150g',            '/images/products/smoked_trout.jpg'),
    ('Lieu noir',      6.99, 4, 'Filet de lieu noir frais, 300g',         '/images/products/black_saithe.jpg'),
    ('Maquereau fumé', 3.99, 4, 'Filets de maquereau fumé, 200g',         '/images/products/smoked_mackerel.jpg'),
    ('Bar frais',      9.49, 4, 'Bar (loup de mer) entier, 400g',         '/images/products/sea_bass.jpg');

-- Boulangerie (cat 5) => produits 62-65
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Pain aux céréales',   2.79, 5, 'Pain multicéréales, 500g',          '/images/products/multigrain_bread.jpg'),
    ('Tarte aux pommes',    5.99, 5, 'Tarte aux pommes maison, 6 parts',  '/images/products/apple_tart.jpg'),
    ('Chausson aux pommes', 1.19, 5, 'Chausson aux pommes feuilleté',     '/images/products/apple_turnover.jpg'),
    ('Pain au chocolat',    1.09, 5, 'Pain au chocolat pur beurre',       '/images/products/pain_au_chocolat.jpg');

-- Boissons (cat 6) => produits 66-68
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Jus de pomme',       1.89, 6, 'Jus de pomme pur fruit, 1L',          '/images/products/apple_juice.jpg'),
    ('Kombucha gingembre', 2.99, 6, 'Kombucha gingembre-citron bio, 330ml', '/images/products/kombucha.jpg'),
    ('Ice tea pêche',      1.69, 6, 'Ice tea saveur pêche, 1.5L',           '/images/products/ice_tea_peach.jpg');

-- Épicerie sèche (cat 7) => produits 69-71
INSERT INTO Product (name, price, categoryId, description, imagePath) VALUES
    ('Quinoa blanc',   3.49, 7, 'Quinoa blanc bio, 500g',              '/images/products/quinoa.jpg'),
    ('Pois chiches',   1.29, 7, 'Pois chiches en boîte, 400g',         '/images/products/chickpeas.jpg'),
    ('Miel d''acacia', 4.99, 7, 'Miel d''acacia pur, pot 350g',        '/images/products/acacia_honey.jpg');
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

INSERT INTO Customer (userId, phoneNumber, address) VALUES
    (7, '0470111222', 'Rue des Lilas 5, 1000 Bruxelles'),
    (8, '0470333444', 'Avenue du Parc 12, 1050 Bruxelles');
GO

-- =============================================
-- TIME SLOTS
-- Créneaux d'1h, 08h–20h, lun–sam (pas de dimanche)
-- Modifie les dates si besoin — ajoute des lignes pour d'autres magasins/jours
-- =============================================

-- Store 1 — jeudi 21/05/2026  (IDs 1–12)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    ('2026-05-21 08:00', '2026-05-21 09:00', 1),
    ('2026-05-21 09:00', '2026-05-21 10:00', 1),
    ('2026-05-21 10:00', '2026-05-21 11:00', 1),
    ('2026-05-21 11:00', '2026-05-21 12:00', 1),
    ('2026-05-21 12:00', '2026-05-21 13:00', 1),
    ('2026-05-21 13:00', '2026-05-21 14:00', 1),
    ('2026-05-21 14:00', '2026-05-21 15:00', 1),
    ('2026-05-21 15:00', '2026-05-21 16:00', 1),
    ('2026-05-21 16:00', '2026-05-21 17:00', 1),
    ('2026-05-21 17:00', '2026-05-21 18:00', 1),
    ('2026-05-21 18:00', '2026-05-21 19:00', 1),
    ('2026-05-21 19:00', '2026-05-21 20:00', 1);

-- Store 1 — vendredi 22/05/2026  (IDs 13–24)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    ('2026-05-22 08:00', '2026-05-22 09:00', 1),
    ('2026-05-22 09:00', '2026-05-22 10:00', 1),
    ('2026-05-22 10:00', '2026-05-22 11:00', 1),
    ('2026-05-22 11:00', '2026-05-22 12:00', 1),
    ('2026-05-22 12:00', '2026-05-22 13:00', 1),
    ('2026-05-22 13:00', '2026-05-22 14:00', 1),
    ('2026-05-22 14:00', '2026-05-22 15:00', 1),
    ('2026-05-22 15:00', '2026-05-22 16:00', 1),
    ('2026-05-22 16:00', '2026-05-22 17:00', 1),
    ('2026-05-22 17:00', '2026-05-22 18:00', 1),
    ('2026-05-22 18:00', '2026-05-22 19:00', 1),
    ('2026-05-22 19:00', '2026-05-22 20:00', 1);

-- Store 1 — samedi 23/05/2026  (IDs 25–36)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    ('2026-05-23 08:00', '2026-05-23 09:00', 1),
    ('2026-05-23 09:00', '2026-05-23 10:00', 1),
    ('2026-05-23 10:00', '2026-05-23 11:00', 1),
    ('2026-05-23 11:00', '2026-05-23 12:00', 1),
    ('2026-05-23 12:00', '2026-05-23 13:00', 1),
    ('2026-05-23 13:00', '2026-05-23 14:00', 1),
    ('2026-05-23 14:00', '2026-05-23 15:00', 1),
    ('2026-05-23 15:00', '2026-05-23 16:00', 1),
    ('2026-05-23 16:00', '2026-05-23 17:00', 1),
    ('2026-05-23 17:00', '2026-05-23 18:00', 1),
    ('2026-05-23 18:00', '2026-05-23 19:00', 1),
    ('2026-05-23 19:00', '2026-05-23 20:00', 1);

-- Store 1 — lundi 25/05/2026  (IDs 37–48)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    ('2026-05-25 08:00', '2026-05-25 09:00', 1),
    ('2026-05-25 09:00', '2026-05-25 10:00', 1),
    ('2026-05-25 10:00', '2026-05-25 11:00', 1),
    ('2026-05-25 11:00', '2026-05-25 12:00', 1),
    ('2026-05-25 12:00', '2026-05-25 13:00', 1),
    ('2026-05-25 13:00', '2026-05-25 14:00', 1),
    ('2026-05-25 14:00', '2026-05-25 15:00', 1),
    ('2026-05-25 15:00', '2026-05-25 16:00', 1),
    ('2026-05-25 16:00', '2026-05-25 17:00', 1),
    ('2026-05-25 17:00', '2026-05-25 18:00', 1),
    ('2026-05-25 18:00', '2026-05-25 19:00', 1),
    ('2026-05-25 19:00', '2026-05-25 20:00', 1);

-- Store 1 — mardi 26/05/2026  (IDs 49–60)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    ('2026-05-26 08:00', '2026-05-26 09:00', 1),
    ('2026-05-26 09:00', '2026-05-26 10:00', 1),
    ('2026-05-26 10:00', '2026-05-26 11:00', 1),
    ('2026-05-26 11:00', '2026-05-26 12:00', 1),
    ('2026-05-26 12:00', '2026-05-26 13:00', 1),
    ('2026-05-26 13:00', '2026-05-26 14:00', 1),
    ('2026-05-26 14:00', '2026-05-26 15:00', 1),
    ('2026-05-26 15:00', '2026-05-26 16:00', 1),
    ('2026-05-26 16:00', '2026-05-26 17:00', 1),
    ('2026-05-26 17:00', '2026-05-26 18:00', 1),
    ('2026-05-26 18:00', '2026-05-26 19:00', 1),
    ('2026-05-26 19:00', '2026-05-26 20:00', 1);

-- Store 2 — jeudi 21/05/2026  (IDs 61–72)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    ('2026-05-21 08:00', '2026-05-21 09:00', 2),
    ('2026-05-21 09:00', '2026-05-21 10:00', 2),
    ('2026-05-21 10:00', '2026-05-21 11:00', 2),
    ('2026-05-21 11:00', '2026-05-21 12:00', 2),
    ('2026-05-21 12:00', '2026-05-21 13:00', 2),
    ('2026-05-21 13:00', '2026-05-21 14:00', 2),
    ('2026-05-21 14:00', '2026-05-21 15:00', 2),
    ('2026-05-21 15:00', '2026-05-21 16:00', 2),
    ('2026-05-21 16:00', '2026-05-21 17:00', 2),
    ('2026-05-21 17:00', '2026-05-21 18:00', 2),
    ('2026-05-21 18:00', '2026-05-21 19:00', 2),
    ('2026-05-21 19:00', '2026-05-21 20:00', 2);

-- Store 3 — jeudi 21/05/2026  (IDs 73–84)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    ('2026-05-21 08:00', '2026-05-21 09:00', 3),
    ('2026-05-21 09:00', '2026-05-21 10:00', 3),
    ('2026-05-21 10:00', '2026-05-21 11:00', 3),
    ('2026-05-21 11:00', '2026-05-21 12:00', 3),
    ('2026-05-21 12:00', '2026-05-21 13:00', 3),
    ('2026-05-21 13:00', '2026-05-21 14:00', 3),
    ('2026-05-21 14:00', '2026-05-21 15:00', 3),
    ('2026-05-21 15:00', '2026-05-21 16:00', 3),
    ('2026-05-21 16:00', '2026-05-21 17:00', 3),
    ('2026-05-21 17:00', '2026-05-21 18:00', 3),
    ('2026-05-21 18:00', '2026-05-21 19:00', 3),
    ('2026-05-21 19:00', '2026-05-21 20:00', 3);

-- Store 4 — jeudi 21/05/2026  (IDs 85–96)
INSERT INTO TimeSlot (startTime, endTime, storeId) VALUES
    ('2026-05-21 08:00', '2026-05-21 09:00', 4),
    ('2026-05-21 09:00', '2026-05-21 10:00', 4),
    ('2026-05-21 10:00', '2026-05-21 11:00', 4),
    ('2026-05-21 11:00', '2026-05-21 12:00', 4),
    ('2026-05-21 12:00', '2026-05-21 13:00', 4),
    ('2026-05-21 13:00', '2026-05-21 14:00', 4),
    ('2026-05-21 14:00', '2026-05-21 15:00', 4),
    ('2026-05-21 15:00', '2026-05-21 16:00', 4),
    ('2026-05-21 16:00', '2026-05-21 17:00', 4),
    ('2026-05-21 17:00', '2026-05-21 18:00', 4),
    ('2026-05-21 18:00', '2026-05-21 19:00', 4),
    ('2026-05-21 19:00', '2026-05-21 20:00', 4);
GO

-- =============================================
-- ORDERS
-- Scénario de démonstration — store 1
--   alice@test.com       → customerId 7
--   bob@test.com         → customerId 8
--   lucas.picker@...     → voit les commandes Pending d'aujourd'hui
--   hugo.cashier@...     → voit toutes les commandes d'aujourd'hui
--
-- IDs des créneaux store 1 :
--   ID  1 = 21/05 08h    ID  3 = 21/05 10h
--   ID  7 = 21/05 14h    ID  9 = 21/05 16h
--   ID 37 = 25/05 08h    ID 43 = 25/05 14h
--   ID 49 = 26/05 08h    ID 51 = 26/05 10h
-- =============================================
INSERT INTO [Order] (orderDate, status, numberOfBoxes, returnedBoxes, pickupDate, paymentStatus, customerId, timeSlotId) VALUES
    --  Alice  |  Pending  |  aujourd'hui 08h  =>  Lucas le voit et le prépare
    (GETDATE(), 'Pending', 0, 0, '2026-05-21 08:00', 'AwaitingPayment', 7,  1),
    --  Alice  |  Ready    |  aujourd'hui 10h  =>  Hugo peut le collecter
    (GETDATE(), 'Ready',   2, 0, '2026-05-21 10:00', 'AwaitingPayment', 7,  3),
    --  Bob    |  Pending  |  aujourd'hui 14h
    (GETDATE(), 'Pending', 0, 0, '2026-05-21 14:00', 'AwaitingPayment', 8,  7),
    --  Bob    |  Pending  |  aujourd'hui 16h
    (GETDATE(), 'Pending', 0, 0, '2026-05-21 16:00', 'AwaitingPayment', 8,  9),
    --  Alice  |  Pending  |  lundi 25/05 08h
    (GETDATE(), 'Pending', 0, 0, '2026-05-25 08:00', 'AwaitingPayment', 7, 37),
    --  Bob    |  Pending  |  lundi 25/05 14h
    (GETDATE(), 'Pending', 0, 0, '2026-05-25 14:00', 'AwaitingPayment', 8, 43),
    --  Alice  |  Pending  |  mardi 26/05 08h
    (GETDATE(), 'Pending', 0, 0, '2026-05-26 08:00', 'AwaitingPayment', 7, 49),
    --  Bob    |  Pending  |  mardi 26/05 10h
    (GETDATE(), 'Pending', 0, 0, '2026-05-26 10:00', 'AwaitingPayment', 8, 51);
GO

-- =============================================
-- ORDER LINES
-- orderId 1 = Alice Pending 21/05 08h
-- orderId 2 = Alice Ready   21/05 10h
-- orderId 3 = Bob   Pending 21/05 14h
-- orderId 4 = Bob   Pending 21/05 16h
-- orderId 5 = Alice Pending 25/05 08h
-- orderId 6 = Bob   Pending 25/05 14h
-- orderId 7 = Alice Pending 26/05 08h
-- orderId 8 = Bob   Pending 26/05 10h
-- =============================================
INSERT INTO OrderLine (orderId, productId, quantity) VALUES
    (1,  1, 3), (1,  8, 2), (1, 40, 1),
    (2, 14, 1), (2, 41, 2), (2, 46, 2),
    (3, 21, 2), (3, 12, 1), (3, 33, 3),
    (4,  3, 4), (4,  9, 3), (4, 38, 1),
    (5,  1, 2), (5,  4, 1), (5, 45, 1),
    (6, 40, 2), (6, 15, 1), (6, 46, 1),
    (7,  6, 1), (7, 14, 1), (7, 13, 1),
    (8,  2, 3), (8,  9, 2), (8, 27, 1);
GO

