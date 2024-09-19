USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_ListarEmpleados
(
	@inUsername VARCHAR(64)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		
		--Se listan los empleados activos
		SELECT E.Nombre, E.ValorDocumentoIdentidad
		FROM dbo.Empleado AS E
		WHERE E.EsActivo = 1

		-- Codigo de salida
		SET @outResult = 0

		SET NOCOUNT OFF;
	END TRY
	BEGIN CATCH
		
		--En caso de que existan errores, se guardan la informacion en una tabla
		INSERT INTO dbo.DBError
		(
			Username
			, ErrorNumber
			, ErrorState
			, ErrorSeverity
			, ErrorLine
			, ErrorProcedure
			, ErrorMessage
			, ErrorDateTime
		)
		VALUES 
		(
			@inUsername
			, ERROR_NUMBER()
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