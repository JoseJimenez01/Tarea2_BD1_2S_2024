﻿USE DBTarea2
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
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		--Se valida que exista el usuario en la base de datos
		IF NOT EXISTS (SELECT 1 FROM dbo.Usuario AS U WHERE U.Username = @inUsername)
		BEGIN
			SET @outResult = 50001; -- username no existe
			RETURN;
		END;

		--En caso de existir el usuario
		INSERT INTO dbo.BitacoraEvento
		(
			idTipoEvento
			, idPostByUser
			, Descripcion
			, PostInIP
			, PostTime
		)
		SELECT 1, U.id, 'Nada', @inPostInIP, GETDATE()
		FROM dbo.Usuario AS U WHERE U.Username = @inUsername;

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