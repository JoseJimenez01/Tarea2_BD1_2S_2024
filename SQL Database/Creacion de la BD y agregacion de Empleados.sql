CREATE DATABASE DBTarea2
GO

USE DBTarea2
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE dbo.Puesto
(
	id INT IDENTITY(1, 1) PRIMARY KEY
	, Nombre VARCHAR(128) NOT NULL
	, SalarioXHora MONEY NOT NULL
);

CREATE TABLE dbo.Empleado
(
	id INT IDENTITY(1,1) PRIMARY KEY
	, idPuesto INT NOT NULL
	, ValorDocumentoIdentidad INT NOT NULL
	, Nombre VARCHAR(128) NOT NULL
	, FehaContratacion DATE NOT NULL
	, SaldoVacaciones MONEY NOT NULL
	, EsActivo BIT NOT NULL
	CONSTRAINT FK_Empleado_Puesto FOREIGN KEY (idPuesto) REFERENCES dbo.Puesto(id)
);

CREATE TABLE dbo.TipoMovimiento
(
	id INT PRIMARY KEY
	, Nombre VARCHAR(32) NOT NULL
	, TipoAccion VARCHAR(32) NOT NULL
);

CREATE TABLE dbo.Usuario
(
	id INT PRIMARY KEY
	, Username VARCHAR(64) NOT NULL
	, Password VARCHAR(64) NOT NULL
);

CREATE TABLE dbo.Movimiento
(
	id INT IDENTITY(1, 1) PRIMARY KEY
	, idEmpleado INT NOT NULL
	, idTipoMovimiento INT NOT NULL
	, idPostByUser INT NOT NULL
	, Fecha DATE NOT NULL
	, Monto MONEY NOT NULL
	, NuevoSaldo MONEY NOT NULL
	, PostInIP VARCHAR(32) NOT NULL
	, PostTime DATETIME NOT NULL
	, FOREIGN KEY (idEmpleado) REFERENCES dbo.Empleado(id)
	, FOREIGN KEY (idTipoMovimiento) REFERENCES dbo.TipoMovimiento(id)
	, FOREIGN KEY (idPostByUser) REFERENCES dbo.Usuario(id)
);

CREATE TABLE dbo.TipoEvento
(
	id INT PRIMARY KEY
	, Nombre VARCHAR(64) NOT NULL
);

CREATE TABLE dbo.BitacoraEvento
(
	id INT IDENTITY(1, 1) PRIMARY KEY
	, idTipoEvento INT NOT NULL
	, idPostByUser INT NOT NULL
	, Descripcion VARCHAR(128) NOT NULL
	, PostInIP VARCHAR(32) NOT NULL
	, PostTime DATETIME NOT NULL
	--CONSTRAINT FK_BitacoraEvento_TipoEvento FOREIGN KEY (idTipoEvento) REFERENCES dbo.TipoEvento(id)
	, FOREIGN KEY (idTipoEvento) REFERENCES dbo.TipoEvento(id)
	--CONSTRAINT FK_BitacoraEvento_Usuario FOREIGN KEY (idPostByUser) REFERENCES dbo.Usuario(id)
	, FOREIGN KEY (idPostByUser) REFERENCES dbo.Usuario(id)
);

CREATE TABLE dbo.DBError(
	ErrorID INT IDENTITY(1,1) NOT NULL,
	UserName VARCHAR(100) NULL,
	ErrorNumber INT NULL,
	ErrorState INT NULL,
	ErrorSeverity INT NULL,
	ErrorLine INT NULL,
	ErrorProcedure VARCHAR(MAX) NULL,
	ErrorMessage VARCHAR(MAX) NULL,
	ErrorDateTime DATETIME NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE dbo.Error
(
	id INT IDENTITY(1, 1) PRIMARY KEY
	, Codigo INT NOT NULL
	, Descripcion VARCHAR(128) NOT NULL
);

