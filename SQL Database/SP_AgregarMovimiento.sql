USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_AgregarMovimiento
(
	@inValorDocIdent INT
	, @inNombre VARCHAR(128)
	, @inSaldoVacaciones MONEY
	, @inMonto MONEY
	, @inTipoMovimiento VARCHAR(32)
	, @inPostInIP VARCHAR(32)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		
		SET NOCOUNT ON;

		--Variables necesarias que se necesitaran a lo largo del SP
		DECLARE @nuevoSaldo MONEY;
		DECLARE @tipoAccion VARCHAR(32);
		DECLARE @idUsuario INT;
		DECLARE @descripcionBitacora VARCHAR(256);
		DECLARE @idEmpleado INT;
		DECLARE @idTipoMovimiento INT;
				
		--Se busca el usuario que esta logueado, por medio del ultimo registro
		--exitoso en la bitacora evento
		SET @idUsuario = (SELECT TOP 1 BE.idPostByUser
							FROM dbo.BitacoraEvento AS BE
							WHERE BE.idTipoEvento = 1
							ORDER BY BE.PostTime DESC);

		-- Para ver que hacemos con el monto recibido, si incrementa o decrementa
		SET @tipoAccion = (SELECT TM.TipoAccion
							FROM dbo.TipoMovimiento AS TM
							WHERE @inTipoMovimiento = TM.Nombre);
		
		--Se setea el saldo para hacer el proceso
		SET @nuevoSaldo = @inSaldoVacaciones;

		IF (@tipoAccion = 'Credito')
		BEGIN
			SET @nuevoSaldo = @nuevoSaldo + @inMonto;
		END
		ELSE IF (@tipoAccion = 'Debito')
		BEGIN
			SET @nuevoSaldo = @nuevoSaldo - @inMonto;
		END;

		--Se valida que no sea negativo el saldo
		IF (@nuevoSaldo < 0)
		BEGIN
			--Seteamos el codigo de error y la descripcion de la bitacora
			SET @outResult = 50011;

			SET @descripcionBitacora = 'Monto del movimiento rechazado pues si se aplicar el saldo seria negativo, ' +
										CONVERT(VARCHAR(32), @inValorDocIdent) + ', ' + @inNombre + ', ' +
										CONVERT(VARCHAR(32), @inSaldoVacaciones) + ', ' + @inTipoMovimiento + ', ' + CONVERT(VARCHAR(32), @inMonto);

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
				13 --Intento de insertar movimiento
				, @idUsuario
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);
			
			RETURN;
		END;

		--Se buscan los valores faltantes para agregar en la tabla movimiento
		SET @idEmpleado = (SELECT E.id
							FROM dbo.Empleado AS E
							WHERE @inNombre = E.Nombre);
		SET @idTipoMovimiento = (SELECT TM.id
								FROM dbo.TipoMovimiento AS TM
								WHERE @inTipoMovimiento = TM.Nombre);

		SET @descripcionBitacora = CONVERT(VARCHAR(32), @inValorDocIdent) + ', ' + @inNombre + ', ' +
										CONVERT(VARCHAR(32), @nuevoSaldo) + ', ' + @inTipoMovimiento + ', ' +
										CONVERT(VARCHAR(32), @inMonto);

		--Si no hubo errores, se guarda la debida informacion
		BEGIN TRANSACTION tAgregarMovimiento;

			--Se actualiza el saldo del empleado
			UPDATE dbo.Empleado
			SET SaldoVacaciones = @nuevoSaldo
			WHERE ValorDocumentoIdentidad = @inValorDocIdent AND Nombre = @inNombre AND EsActivo = 1;

			--Se agrega el movimiento
			INSERT INTO dbo.Movimiento
			(
				idEmpleado
				, idTipoMovimiento
				, idPostByUser
				, Fecha
				, Monto
				, NuevoSaldo
				, PostInIP
				, PostTime
			)
			VALUES
			(
				@idEmpleado
				, @idTipoMovimiento
				, @idUsuario
				, CONVERT (DATE, GETDATE())
				, @inMonto
				, @nuevoSaldo
				, @inPostInIP
				, GETDATE()
			)

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
				14 --insertar movimiento exitoso
				, @idUsuario
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);
				
		COMMIT TRANSACTION tAgregarMovimiento;

		SET @outResult = 0;

		SET NOCOUNT OFF;
	END TRY
	BEGIN CATCH
		
		-- Se deshace lo hecho en la transación
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION tAgregarMovimiento;
		END

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