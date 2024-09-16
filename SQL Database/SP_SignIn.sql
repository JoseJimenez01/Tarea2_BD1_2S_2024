USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_SignIn
(
	@inUsername VARCHAR(64)
	, @inPassword VARCHAR(64)
	, @inPostInIP VARCHAR(32)
	, @outMessage VARCHAR(128) OUTPUT
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		--Se valida que exista el usuario en la base de datos
		IF NOT EXISTS (SELECT 1 FROM dbo.Usuario AS U WHERE U.Username = @inUsername)
		BEGIN
			SET @outResult = 50001;
			RETURN;
		END;

		--En caso de no haberlo, se agrega el empleado
		INSERT INTO dbo.Empleado(
			Nombre
			, Salario)
		VALUES(
			@inNombre
			, @inSalario
		);
    
		--Se guardan los valores de salida del SP
		SET @outResult = 0
		SET @outMessage = 'Empleados agregados exitosamente.'

		SET NOCOUNT OFF;
	END TRY
	BEGIN CATCH
		
		--En caso de que existan errores, se guardan la informaci�n en una tabla
		INSERT INTO dbo.DBErrors
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

		--Se guardan los valores de salida del SP
		SET @outResult = 50005
		SET @outMessage = ERROR_MESSAGE()
		SET NOCOUNT OFF;
	END CATCH
END