CREATE TABLE [dbo].[Productos]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Nombre] NCHAR(20) NULL, 
    [Cantidad] FLOAT NOT NULL DEFAULT 0.0, 
    [Precio] MONEY NOT NULL DEFAULT 0.0, 
    [Proveedor] NCHAR(20) NULL
)
