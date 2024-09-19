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
	, @inCantIntentosSesionFallidos INT
	, @inPostInIP VARCHAR(32)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		
		DECLARE @codTipoEvento INT;
		DECLARE @descripcionBitacora VARCHAR(64);

		SET NOCOUNT ON;

		--Se valida que exista el usuario en la base de datos
		IF NOT EXISTS (SELECT 1 FROM dbo.Usuario AS U WHERE U.Username = @inUsername)
		BEGIN
			SET @outResult = 50001; -- username no existe
		END
		--Se valida que exista la contraseña en la base de datos
		ELSE IF NOT EXISTS (SELECT 1 FROM dbo.Usuario AS U WHERE U.Password = @inPassword)
		BEGIN
			SET @outResult = 50002; -- password no existe
		END
		-- Codigo de salida
		ELSE
		BEGIN
			SET @outResult = 0
		END;
		
		--Seteamos el valor del tipo de evento y la descripcion de la bitacora segun codigo de salida
		IF @outResult != 0
		BEGIN
			SET @codTipoEvento = 2;
			SET @descripcionBitacora = 'Inicio de sesion fallidos en 20 min: ' + CONVERT(VARCHAR(5), @inCantIntentosSesionFallidos) + ', codigo de error: ' + CONVERT(VARCHAR(5), @outResult);
		END
		ELSE
		BEGIN
			SET @codTipoEvento = 1;
			SET @descripcionBitacora = 'Nada'
		END;

		--En caso de existir el usuario y contraseña
		IF (@outResult = 0)
		BEGIN
			INSERT INTO dbo.BitacoraEvento
			(
				idTipoEvento
				, idPostByUser
				, Descripcion
				, PostInIP
				, PostTime
			)
			SELECT @codTipoEvento, U.id, @descripcionBitacora, @inPostInIP, GETDATE()
			FROM dbo.Usuario AS U WHERE U.Username = @inUsername;
		END
		-- En caso de que no exista, se guardara 7 para un usuario no existente
		ELSE
		BEGIN
			INSERT INTO dbo.BitacoraEvento
			(
				idTipoEvento
				, idPostByUser
				, Descripcion
				, PostInIP
				, PostTime
			)
			VALUES
			(
				@codTipoEvento
				, 7
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);
		END;

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