USE [linea_prod_2026_07_09];
GO

IF OBJECT_ID('[dbo].[DistinteBase]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DistinteBase];
GO

CREATE TABLE [dbo].[DistinteBase] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [IdUtente]    BIGINT         NOT NULL,
    [Codice]      NVARCHAR (12)  NOT NULL,
    [Descrizione] NVARCHAR (512) NULL,
    [DataInizio]  DATETIME2 (7)  NOT NULL,
    [Versione]    NVARCHAR (12)  NOT NULL,
    [Stato]       NVARCHAR (64)  NULL,
    
    -- Chiave Primaria con ordinamento crescente (Clustered Index)
    CONSTRAINT [PK_DistinteBase] PRIMARY KEY CLUSTERED ([Id] ASC),
    
    -- Vincolo Unico composto tra Codice e Versione
    CONSTRAINT [UQ_DistinteBase_Codice_Versione] UNIQUE NONCLUSTERED ([Codice] ASC, [Versione] ASC)
);
GO