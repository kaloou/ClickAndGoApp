USE master;
GO

IF DB_ID('ClickAndCollect_Farhane_Paludetto') IS NOT NULL
BEGIN
    ALTER DATABASE ClickAndCollect_Farhane_Paludetto SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ClickAndCollect_Farhane_Paludetto;
END
GO

CREATE DATABASE ClickAndCollect_Farhane_Paludetto;
GO