USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_ListarMovimientos
(
	@inNombreEmpleado VARCHAR(128)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--Buscamos el nombre del empleado
		DECLARE @idEmpleado INT;
		SET @idEmpleado = (SELECT E.id
							FROM dbo.Empleado AS E
							WHERE E.Nombre = @inNombreEmpleado)

		--Se lista el empleado requerido
		SELECT E.Nombre, E.ValorDocumentoIdentidad, E.SaldoVacaciones
		FROM dbo.Empleado AS E
		WHERE (E.EsActivo = 1) AND (E.Nombre = @inNombreEmpleado);

		--Se lista los movimientos requeridos
		SELECT M.Fecha, TM.Nombre, M.Monto, M.NuevoSaldo, U.Username, M.PostInIP, M.PostTime
		FROM dbo.Movimiento AS M
		INNER JOIN dbo.TipoMovimiento AS TM
		ON M.idTipoMovimiento = TM.id
		INNER JOIN dbo.Usuario AS U
		ON M.idPostByUser = U.id
		WHERE @idEmpleado = M.idEmpleado
		ORDER BY M.Fecha DESC;

		-- Codigo de salida
		SET @outResult = 0

		SET NOCOUNT OFF;
	END TRY
	BEGIN CATCH
		
		--En caso de que existan errores, se guardan la informacion en una tabla
		INSERT INTO dbo.DBError
		(
			ErrorNumber
			, ErrorState
			, ErrorSeverity
			, ErrorLine
			, ErrorProcedure
			, ErrorMessage
			, ErrorDateTime
		)
		VALUES 
		(
			ERROR_NUMBER()
			, ERROR_STATE()
			, ERROR_SEVERITY()
			, ERROR_LINE()
			, ERROR_PROCEDURE()
			, ERROR_MESSAGE()
			, GETDATE()
		);

		-- Codigo de salida
		SET @outResult = 50008 --Error generado en la base de datos
		SET NOCOUNT OFF;
	END CATCH
END