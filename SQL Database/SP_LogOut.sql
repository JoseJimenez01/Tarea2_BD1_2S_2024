USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_LogOut
(
	@inPostInIP VARCHAR(32)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		
		SET NOCOUNT ON;

		--Buscamos el id por el ultimo inicio de sesion exitoso
		DECLARE @idUsuario INT;

		SET @idUsuario = (SELECT TOP 1 BE.idPostByUser
							FROM dbo.BitacoraEvento AS BE
							WHERE BE.idTipoEvento = 1
							ORDER BY BE.PostTime DESC);

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
			4 --LogOut
			, @idUsuario
			, 'Nada'
			, @inPostInIP
			, GETDATE()
		);
		
		SET @outResult = 0;

		SET NOCOUNT OFF;
	END TRY
	BEGIN CATCH
		
		--Buscamos el username para DBError
		DECLARE @username VARCHAR(64);
		
		SET @username = (SELECT U.Username
		FROM dbo.Usuario AS U
		WHERE U.id = @idUsuario)

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
			@username
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