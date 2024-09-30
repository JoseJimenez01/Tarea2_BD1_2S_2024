USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_AgregarEmpleado
(
	@inValorDocIdent INT
	, @inNombre VARCHAR(128)
	, @inPuesto VARCHAR(128)
	, @inNombreEsAlfabetico INT -- 1 si es alfabetico, 2 si no lo es
	, @inValorDocNoEsAlfabetico INT -- 1 si no es alfabetico, 2 si es
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

		SET @outResult = 0;

		--Valida si el nombre no es alfabetico
		IF (@inNombreEsAlfabetico = 2)
		BEGIN
			SET @outResult = 50009; --nombre no alfabetico
		END
		--Valida si el valor del documento de identidad es alfabetico
		ELSE IF (@inValorDocNoEsAlfabetico = 2)
		BEGIN
			SET @outResult = 50010; --valorDocIdent alfabetico
		END
		--Valida si el nombre ya existe en la base de datos
	   	IF EXISTS (SELECT 1 FROM dbo.Empleado AS E WHERE E.Nombre = @inNombre)
		BEGIN
			SET @outResult = 50005; -- nombre empleado ya existe en insercion
		END
		--Valida si el valor del doc ident ya existe
		ELSE IF EXISTS (SELECT 1 FROM dbo.Empleado AS E WHERE E.ValorDocumentoIdentidad = @inValorDocIdent)
		BEGIN
			SET @outResult = 50004; -- valor doc ident ya existe en inserción
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
			SET @descripcionBitacora = @descripcionBitacora + ', ' + @inValorDocIdent + ', ' + @inNombre + ', ' + @inPuesto;

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
				5 -- ya que hubo errores, es una insercion no exitosa
				, @idUsuario
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);
			RETURN;
		END;
		
		-- Una vez validados los datos, se actualizan las tablas
		BEGIN TRANSACTION tAgregarEmpelado;

			SET @idPuesto = (SELECT P.id
							FROM dbo.Puesto AS P
							WHERE P.Nombre = @inPuesto);

			-- Se inserta el nuevo empleado
			INSERT INTO dbo.Empleado
			(
				idPuesto
				, ValorDocumentoIdentidad
				, Nombre
				, FechaContratacion
				, SaldoVacaciones
				, EsActivo
			)
			VALUES
			(
				@idPuesto
				, @inValorDocIdent
				, @inNombre
				, CONVERT (DATE, GETDATE())
				, 0.00 --por defecto cero
				, 1 --por defecto 1
			);

			SET @descripcionBitacora = @inValorDocIdent + ', ' + @inNombre + ', ' + @inPuesto;

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
				6 -- insercion exitosa
				, @idUsuario
				, @descripcionBitacora
				, @inPostInIP
				, GETDATE()
			);

		COMMIT TRANSACTION tAgregarEmpelado;

		SET NOCOUNT OFF;
	END TRY
	BEGIN CATCH
		
		-- Se deshace lo hecho en la transación
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION tAgregarEmpelado;
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