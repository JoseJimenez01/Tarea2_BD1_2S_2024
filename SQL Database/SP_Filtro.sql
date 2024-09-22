USE DBTarea2
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_Filtro
(
	--se usaran el valor ingresado nombre y valorDocIdent como string
	--aunque el valorDocIdent sea int
	@inStringFiltro VARCHAR(128)
	, @inNameOrValueDoc INT -- 11 si nombre, 12 si valorDoc....
	, @inPostInIP VARCHAR(32)
	, @outResult INT OUTPUT
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		--Si son espacios en blanco entonces se muestran todos los empleados
		IF (@inStringFiltro = ' ')
		BEGIN
			SELECT E.Nombre, E.ValorDocumentoIdentidad
			FROM dbo.Empleado AS E
			WHERE E.EsActivo = 1
			ORDER BY E.Nombre ASC
		END
		ELSE
		BEGIN
			--Se listan los empleados activos y filtrados
			SELECT E.Nombre, E.ValorDocumentoIdentidad
			FROM dbo.Empleado AS E
			WHERE E.EsActivo = 1 AND ((UPPER(E.Nombre) LIKE UPPER('%'+@inStringFiltro+'%') OR CONVERT(VARCHAR(128), E.ValorDocumentoIdentidad) LIKE '%'+@inStringFiltro+'%'))
			ORDER BY E.Nombre ASC
		END;

		--Se busca el usuario que esta logueado, por medio del ultimo registro
		--exitoso en la bitacora evento
		DECLARE @idUsuario INT;

		SET @idUsuario = (SELECT TOP 1 BE.idPostByUser
							FROM dbo.BitacoraEvento AS BE
							WHERE BE.idTipoEvento = 1
							ORDER BY BE.PostTime DESC);

		--Se guarda la informacion en la bitacora de evento
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
			@inNameOrValueDoc
			, @idUsuario
			, @inStringFiltro
			, @inPostInIP
			, GETDATE()
		);

		-- Codigo de salida
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
			UserName
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
			,ERROR_NUMBER()
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