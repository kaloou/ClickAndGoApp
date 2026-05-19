USE master;
GO

IF DB_ID('ClickAndCollect_Farhane_Paludetto') IS NOT NULL
BEGIN
    ALTER DATABASE ClickAndCollect_Farhane_Paludetto SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ClickAndCollect_Farhane_Paludetto;
    CREATE DATABASE ClickAndCollect_Farhane_Paludetto;
END

IF DB_ID('ClickAndCollect_Farhane_Binome') IS NOT NULL
    BEGIN
        ALTER DATABASE ClickAndCollect_Farhane_Paludetto SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE ClickAndCollect_Farhane_Paludetto;
        CREATE DATABASE ClickAndCollect_Farhane_Binome;
    END
GO

GO