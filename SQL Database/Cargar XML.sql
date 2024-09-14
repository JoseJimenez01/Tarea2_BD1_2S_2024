
USE DBTarea2
GO

-- Se declara la variable que contendrá los datos XML
DECLARE @xmlData XML;

-- SE cargan los datos XML a la variable, cambiar la ruta según sea el caso
SET @xmlData = (
		SELECT *
		FROM OPENROWSET(BULK 'C:\Users\Usuario II 2024\Desktop\Tarea2_BD1_2S_2024\SQL Database\Datos.xml', SINGLE_BLOB) 
		AS xmlData
		);

-- Tabla para la manipulación y mapeo de los datos para la tabla Empleado
DECLARE @TablaVariableEmpleado TABLE
(
    Puesto VARCHAR(128) NOT NULL
    , ValDocIdent INT NOT NULL
    , Nombre VARCHAR(128) NOT NULL
    , FechaContrat DATE  NOT NULL
);

-- Tabla para la manipulación y mapeo de los datos para la tabla Empleado
DECLARE @TablaVariableMovimiento TABLE
(
    ValorDocIdent INT NOT NULL
    , TipoMovim VARCHAR(32) NOT NULL
    , Fecha DATE  NOT NULL
	, Monto MONEY NOT NULL
	, PostByUser VARCHAR(64) NOT NULL
	, PostInIP VARCHAR(32) NOT NULL
	, PostTime DATETIME NOT NULL
);

-- Se insertan los datos a la tala dbo.Puesto---------------------------------------------------
INSERT INTO dbo.Puesto(
	Nombre
	, SalarioXHora)
SELECT  
	T.Item.value('@Nombre', 'VARCHAR(128)')
	, T.Item.value('@SalarioxHora', 'MONEY')
FROM @xmlData.nodes('Datos/Puestos/Puesto') AS T(Item)

-- Se insertan los datos a la tala dbo.TipoEvento-----------------------------------------------
INSERT INTO dbo.TipoEvento(
	id
	, Nombre)
SELECT
	T.item.value('@Id', 'INT')
	, T.item.value('@Nombre', 'VARCHAR(64)')
FROM @xmlData.nodes('Datos/TiposEvento/TipoEvento') AS T(Item)

-- Se insertan los datos a la tala dbo.TipoMovimiento ------------------------------------------------
INSERT INTO dbo.TipoMovimiento(
	id
	, Nombre
	, TipoAccion)
SELECT
	T.item.value('@Id', 'INT')
	, T.item.value('@Nombre', 'VARCHAR(32)')
	, T.item.value('@TipoAccion', 'VARCHAR(32)')
FROM @xmlData.nodes('Datos/TiposMovimientos/TipoMovimiento') AS T(Item)

-- Se insertan los datos a la tala TablaVariableEmpleado para mapear el puesto y -------------------------
-- luego agregar la info a la tabla dbo.Empleado
INSERT INTO @TablaVariableEmpleado(
	Puesto
    , ValDocIdent
    , Nombre
    , FechaContrat)
SELECT
	T.item.value('@Puesto', 'VARCHAR(128)')
	, T.item.value('@ValorDocumentoIdentidad', 'INT')
	, T.item.value('@Nombre', 'VARCHAR(128)')
	, T.item.value('@FechaContratacion', 'DATE')
FROM @xmlData.nodes('Datos/Empleados/empleado') AS T(Item)

-- Ahora se hace la union entre tablas para buscar el valor del id del puesto
INSERT INTO dbo.Empleado(
	idPuesto
	, ValorDocumentoIdentidad
	, Nombre
	, FehaContratacion
	, SaldoVacaciones
	, EsActivo)
SELECT P.id, TV.ValDocIdent, TV.Nombre, TV.FechaContrat, 0.00, 1
FROM dbo.Puesto AS P
INNER JOIN @TablaVariableEmpleado AS TV
ON P.Nombre = TV.Puesto;

-- Se insertan los datos a la tala dbo.Usuario ------------------------------------------------
INSERT INTO dbo.Usuario(
	id
	, Username
	, Password)
SELECT
	T.item.value('@Id', 'INT')
	, T.item.value('@Nombre', 'VARCHAR(64)')
	, T.item.value('@Pass', 'VARCHAR(64)')
FROM @xmlData.nodes('Datos/Usuarios/usuario') AS T(Item)

-- Se insertan los datos a la tala TablaVariableMovimiento para mapear el idEmpleado, -------------------------------
-- el idTipoMovimiento y el idPostByUser, luego agregar la info a la tabla dbo.Movimiento
INSERT INTO @TablaVariableMovimiento(
	ValorDocIdent
    , TipoMovim
    , Fecha
	, Monto
	, PostByUser
	, PostInIP
	, PostTime)
SELECT
	T.item.value('@ValorDocId', 'INT')
	, T.item.value('@IdTipoMovimiento', 'VARCHAR(32)')
	, T.item.value('@Fecha', 'DATE')
	, T.item.value('@Monto', 'MONEY')
	, T.item.value('@PostByUser', 'VARCHAR(64)')
	, T.item.value('@PostInIP', 'VARCHAR(32)')
	, T.item.value('@PostTime', 'DATETIME')
FROM @xmlData.nodes('Datos/Movimientos/movimiento') AS T(Item)

-- Ahora se hace la union entre tablas para buscar el valor del id del empleado,
-- del usuario y del tipo de movimiento
INSERT INTO dbo.Movimiento(
	idEmpleado
	, idTipoMovimiento
	, idPostByUser
	, Fecha
	, Monto
	, NuevoSaldo
	, PostInIP
	, PostTime)
SELECT E.id, TM.id, U.id, TVM.Fecha, TVM.Monto, 0.00, TVM.PostInIP, TVM.PostTime
FROM dbo.Empleado AS E
INNER JOIN @TablaVariableMovimiento AS TVM
ON E.ValorDocumentoIdentidad = TVM.ValorDocIdent
INNER JOIN dbo.TipoMovimiento AS TM
ON TM.Nombre = TVM.TipoMovim
INNER JOIN dbo.Usuario AS U
ON U.Username = TVM.PostByUser;

-- Se insertan los datos a la tala dbo.Error ------------------------------------------------
INSERT INTO dbo.Error(
	Codigo
	, Descripcion)
SELECT
	T.item.value('@Codigo', 'INT')
	, T.item.value('@Descripcion', 'VARCHAR(128)')
FROM @xmlData.nodes('Datos/Error/error') AS T(Item)

