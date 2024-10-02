USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_ConsultaError
(
	@inCodigo VARCHAR(128)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		
		SELECT E.Descripcion
		FROM dbo.Error AS E
		WHERE @inCodigo = E.Codigo;

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