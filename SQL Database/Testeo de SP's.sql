USE DBTarea2
GO
--Testeo del sp para consultar codigo de error
DECLARE @username VARCHAR(64), @codigo VARCHAR(64), @result INT

EXEC dbo.SP_ConsultaError 'JosePrueba', '50002', @result OUTPUT

SELECT @result


--Testeo del sp para agregar empleados
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_ConsultaInicioDeSesionFallidos 'JosePrueba', 20, '192.01.09', @result OUTPUT

SELECT @result

--Testeo del sp para filtrar empleados
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_Filtro 'nav', 12, '192.01.09', @result OUTPUT

SELECT @result

--Testeo del sp para consultar los puestos
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_ListarPuestos @result OUTPUT

SELECT @result

--Testeo del sp para agregar empleados
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_AgregarEmpleado 9999997, 'JosePrueba3', 'Cajero', 1, 1, '111.111.1.1111', @result OUTPUT

SELECT @result

--Testeo del sp para agregar empleados
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_ListarMovimientos 'Christina Ward', @result OUTPUT

SELECT @result