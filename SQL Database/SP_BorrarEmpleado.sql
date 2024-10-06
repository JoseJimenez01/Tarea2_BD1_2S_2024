USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_BorrarEmpleado
(
	@inNombreEmpleado VARCHAR(128)
	, @inValorDocIdent INT
	, @inPuesto VARCHAR(128)
	, @inSaldoVacaciones DECIMAL
	, @inConfirmaOno INT -- 1 se el usuario confirma, 2 si no
	, @inPostInIP VARCHAR(32)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		DECLARE @idUsuario INT;
		DECLARE @descripcionBitacora VARCHAR(256);
				
		--Se busca el usuario que esta logueado, por medio del ultimo registro
		--exitoso en la bitacora evento
		SET @idUsuario = (SELECT TOP 1 BE.idPostByUser
							FROM dbo.BitacoraEvento AS BE
							WHERE BE.idTipoEvento = 1
							ORDER BY BE.PostTime DESC);
		
		SET @descripcionBitacora = CONVERT(VARCHAR(32), @inValorDocIdent) + ', ' + @inNombreEmpleado +
									', ' + @inPuesto + ', ' + CONVERT(VARCHAR(32), @inSaldoVacaciones);

		IF (@inConfirmaOno = 2)
		BEGIN

			--Se registra el intento de agregar movimiento
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
				9 -- intento de borrado
				, @idUsuario
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);
			SET @outResult = 0;

			RETURN;
		END;

		--Se realiza la transaccion
		BEGIN TRANSACTION tBorrarEmpleado;

			--Se borra de manera logica
			UPDATE dbo.Empleado
			SET EsActivo = 0
			WHERE Nombre = @inNombreEmpleado AND EsActivo = 1;

			--Se registra el intento de agregar movimiento
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
				10 --borrado exitoso
				, @idUsuario
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);
 
		COMMIT TRANSACTION tBorrarEmpleado;
		
		-- Codigo de salida
		SET @outResult = 0

		SET NOCOUNT OFF;
	END TRY
	BEGIN CATCH
		
		-- Se deshace lo hecho en la transación
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION tBorrarEmpleado;
		END

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