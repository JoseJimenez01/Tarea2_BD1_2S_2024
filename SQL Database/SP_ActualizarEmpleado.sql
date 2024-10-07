USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_ActualizarEmpleado
(
	@inValorDocIdentOriginal INT
	, @inNombreOriginal VARCHAR(128)
	, @inPuestoOriginal VARCHAR(128)
	, @inValorDocIdent VARCHAR(128)		--valor nuevo
	, @inNombre VARCHAR(128)	--nombre nuevo
	, @inPuesto VARCHAR(128)	--puesto nuevo
	, @inNombreEsAlfabetico INT -- 1 si es alfabetico, 2 si no lo es
	, @inPostInIP VARCHAR(32)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		
		SET NOCOUNT ON;

		DECLARE @idUsuario INT;
		DECLARE @descripcionBitacora VARCHAR(256);
		DECLARE @idPuesto INT;
		DECLARE @saldoVacas MONEY;
		DECLARE @valorDocEntero INT;

		--Se pone al inicio por temas de validacion, y el valor recibido al llamar al SP
		SET @outResult = 0;

		--Tratamos de convertir el valorDoc a un entero
		SET @valorDocEntero = CASE 
                WHEN TRY_CONVERT(INT, @inValorDocIdent) IS NOT NULL 
                    THEN TRY_CONVERT(INT, @inValorDocIdent)
                ELSE 2  -- Si la conversión falla, asigna 0
            END

		--Valida si el nombre no es alfabetico
		IF (@inNombreEsAlfabetico = 2)
		BEGIN
			SET @outResult = 50009; --nombre no alfabetico
		END
		--Valida si el valor del documento de identidad es alfabetico
		ELSE IF (@valorDocEntero = 2)
		BEGIN
			SET @outResult = 50010; --valorDocIdent es alfabetico
		END
		--Valida si el nombre ya existe en la base de datos
	   	ELSE IF EXISTS (SELECT 1 FROM dbo.Empleado AS E WHERE E.Nombre = @inNombre)
		BEGIN
			SET @outResult = 50007; -- nombre empleado ya existe en actualizacion
		END
		--Valida si el valor del doc ident ya existe
		ELSE IF EXISTS (SELECT 1 FROM dbo.Empleado AS E WHERE E.ValorDocumentoIdentidad = @inValorDocIdent)
		BEGIN
			SET @outResult = 50006; -- valor doc ident ya existe en actualizacion
		END;

		--Se busca el usuario que esta logueado, por medio del ultimo registro
		--exitoso en la bitacora evento
		SET @idUsuario = (SELECT TOP 1 BE.idPostByUser
							FROM dbo.BitacoraEvento AS BE
							WHERE BE.idTipoEvento = 1
							ORDER BY BE.PostTime DESC);

		--Valida si hubo errores para agregar registro a la bitácora y salir
		IF (@outResult != 0)
		BEGIN
			-- Guardamos el error primero
			SET @descripcionBitacora = (SELECT E.Descripcion
										FROM dbo.Error AS E
										WHERE E.Codigo = @outResult);
			-- Luego guardamos la info
			SET @descripcionBitacora = @descripcionBitacora + ', ' + CONVERT(VARCHAR(32), @inValorDocIdentOriginal) + ', ' +
										@inNombreOriginal + ', ' + @inPuestoOriginal + ', ' +  CONVERT(VARCHAR(32), @inValorDocIdent) +
										', ' + @inNombre + ', ' + @inPuesto;

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
				7 -- ya que hubo errores, update no exitoso
				, @idUsuario
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);
			RETURN;
		END;
		
		--Se buscan los valores faltantes para ingresar en las tablas
		SET @idPuesto = (SELECT P.id
							FROM dbo.Puesto AS P
							WHERE P.Nombre = @inPuesto);
		
		SET @saldoVacas = (SELECT E.SaldoVacaciones
							FROM dbo.Empleado AS E
							WHERE E.Nombre = @inNombreOriginal);

		SET @descripcionBitacora = CONVERT(VARCHAR(32), @inValorDocIdentOriginal) + ', ' +
									@inNombreOriginal + ', ' + @inPuestoOriginal + ', ' + CONVERT(VARCHAR(32), @inValorDocIdent) +
									', ' + @inNombre + ', ' + @inPuesto + ', ' + CONVERT(VARCHAR(32), @saldoVacas);

		-- Una vez validados los datos, se actualizan las tablas
		BEGIN TRANSACTION tActualizarEmpleado;

			-- Se actualiza el empleado indicado
			UPDATE dbo.Empleado
			SET ValorDocumentoIdentidad = @inValorDocIdent,
				Nombre = @inNombre,
				idPuesto = @idPuesto
			WHERE ValorDocumentoIdentidad = @inValorDocIdentOriginal AND Nombre = @inNombreOriginal AND EsActivo = 1;

			-- Luego se registra la inserción en la bitácora
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
				8 -- update exitoso
				, @idUsuario
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);

		COMMIT TRANSACTION tActualizarEmpleado;

		SET NOCOUNT OFF;
	END TRY
	BEGIN CATCH
		
		-- Se deshace lo hecho en la transación
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION tActualizarEmpleado;
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