CREATE TABLE [dbo].[Productos] (
    [Id]        INT        NOT NULL,
    [Nombre]    NCHAR (20) NULL,
    [Cantidad]  FLOAT (53) DEFAULT ((0.0)) NOT NULL,
    [Precio]    MONEY      DEFAULT ((0.0)) NOT NULL,
    [Proveedor] NCHAR (20) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

