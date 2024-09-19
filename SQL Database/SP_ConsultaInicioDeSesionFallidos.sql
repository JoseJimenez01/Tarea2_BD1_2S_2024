USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_ConsultaInicioDeSesionFallidos
(
	@inUsername VARCHAR(64)
	, @in20minOr30min INT
	, @inPostInIP VARCHAR(32)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		
		DECLARE @cantFallos INT;
		--Consultamos la cantidad de intentos de inicio de sesion fallidos
		-- ya sea en 20min o 30min pasados
		IF (@in20minOr30min = 20)
		BEGIN
			SELECT @cantFallos =  COUNT(*)
			FROM dbo.BitacoraEvento AS BE
			WHERE BE.idTipoEvento = 2 AND BE.PostTime >= DATEADD(MINUTE, -20, GETDATE());
			
			SELECT @cantFallos;
		END
		ELSE IF (@in20minOr30min = 30)
		BEGIN
			SELECT @cantFallos =  COUNT(*)
			FROM dbo.BitacoraEvento AS BE
			WHERE BE.idTipoEvento = 2 AND BE.PostTime >= DATEADD(MINUTE, -30, GETDATE());

			SELECT @cantFallos;

			-- Cuando fallos > 5
			IF (@cantFallos > 5)
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
					3
					, 7
					, 'Nada'
					, @inPostInIP
					, GETDATE()
				);
			END;
		END;

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