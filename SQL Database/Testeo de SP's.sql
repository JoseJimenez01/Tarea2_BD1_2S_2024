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