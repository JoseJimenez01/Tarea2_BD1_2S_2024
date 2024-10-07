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

--Testeo del sp para agregar empleados
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_AgregarMovimiento 111111, 'Jose prueba desde el navegador', 0.00, 1000000.00, 'Reversion Debito', '111.111.1.1111', @result OUTPUT

SELECT @result


--Testeo del sp para agregar empleados
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_SacarEmpleado 'Jose Jimenez', @result OUTPUT

SELECT @result


--Testeo del sp para agregar empleados
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_ActualizarEmpleado 5095109, 'Christina Ward', 'Cajero', 50951096, 'Christina Ward Jhonson', 'Conserje', 1, 1, '111.111.1.1111', @result OUTPUT

SELECT @result 



--Testeo del sp para borrar empleados
USE DBTarea2
GO
DECLARE @result INT

EXEC dbo.SP_BorrarEmpleado 'Christina Ward Jhonson', 50951096, 'Conserje', 0.00, 1, '111.111.1.1111', @result OUTPUT

SELECT @result 

