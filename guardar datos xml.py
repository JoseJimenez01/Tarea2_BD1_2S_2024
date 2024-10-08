
datos = '''<Datos> 
<Puestos> 
	<Puesto Nombre="Cajero" SalarioxHora="11.00"/> 
	<Puesto Nombre="Camarero" SalarioxHora="10.00"/> 
	<Puesto Nombre="Cuidador" SalarioxHora="13.50"/> 
	<Puesto Nombre="Conductor" SalarioxHora="15.00"/> 
	<Puesto Nombre="Asistente" SalarioxHora="11.00"/> 
	<Puesto Nombre="Recepcionista" SalarioxHora="12.00"/> 
	<Puesto Nombre="Fontanero" SalarioxHora="13.00"/> 
	<Puesto Nombre="Niñera" SalarioxHora="12.00"/> 
	<Puesto Nombre="Conserje" SalarioxHora="11.00"/> 
	<Puesto Nombre="Albañil" SalarioxHora="10.50"/> 
</Puestos> 
<TiposEvento> 
	<TipoEvento Id="1" Nombre="Login Exitoso"/> 
	<TipoEvento Id="2" Nombre="Login No Exitoso"/> 
	<TipoEvento Id="3" Nombre="Login deshabilitado"/> 
	<TipoEvento Id="4" Nombre="Logout"/> 
	<TipoEvento Id="5" Nombre="Insercion no exitosa"/> 
	<TipoEvento Id="6" Nombre="Insercion exitosa"/> 
	<TipoEvento Id="7" Nombre="Update no exitoso"/> 
	<TipoEvento Id="8" Nombre="Update exitoso"/> 
	<TipoEvento Id="9" Nombre="Intento de borrado"/> 
	<TipoEvento Id="10" Nombre="Borrado exitoso"/> 
	<TipoEvento Id="11" Nombre="Consulta con filtro de nombre"/> 
	<TipoEvento Id="12" Nombre="Consulta con filtro de cedula"/> 
	<TipoEvento Id="13" Nombre="Intento de insertar movimiento"/> 
	<TipoEvento Id="14" Nombre="Insertar movimiento exitoso"/> 
</TiposEvento> 
<TiposMovimientos> 
	<TipoMovimiento Id="1" Nombre="Cumplir mes" TipoAccion="Credito"/> 
	<TipoMovimiento Id="2" Nombre="Bono vacacional" TipoAccion="Credito"/> 
	<TipoMovimiento Id="3" Nombre="Reversion Debito" TipoAccion="Credito"/> 
	<TipoMovimiento Id="4" Nombre="Disfrute de vacaciones" TipoAccion="Debito"/> 
	<TipoMovimiento Id="5" Nombre="Venta de vacaciones" TipoAccion="Debito"/> 
	<TipoMovimiento Id="6" Nombre="Reversion de Credito" TipoAccion="Debito"/> 
</TiposMovimientos> 
<Empleados> 
	<empleado Puesto="Camarero" ValorDocumentoIdentidad="6993943" Nombre="Kaitlyn Jensen" FechaContratacion="2017-12-07"/> 
	<empleado Puesto="Albañil" ValorDocumentoIdentidad="1896802" Nombre="Robert Buchanan" FechaContratacion="2020-09-20"/> 
	<empleado Puesto="Cajero" ValorDocumentoIdentidad="5095109" Nombre="Christina Ward" FechaContratacion="2015-09-13"/> 
	<empleado Puesto="Fontanero" ValorDocumentoIdentidad="8403646" Nombre="Bradley Wright" FechaContratacion="2020-01-27"/> 
	<empleado Puesto="Conserje" ValorDocumentoIdentidad="6019592" Nombre="Robert Singh" FechaContratacion="2017-02-01"/> 
	<empleado Puesto="Asistente" ValorDocumentoIdentidad="4510358" Nombre="Ryan Mitchell" FechaContratacion="2018-06-08"/> 
	<empleado Puesto="Asistente" ValorDocumentoIdentidad="7517662" Nombre="Candace Fox" FechaContratacion="2013-12-17"/> 
	<empleado Puesto="Asistente" ValorDocumentoIdentidad="8326328" Nombre="Allison Murillo" FechaContratacion="2020-04-19"/> 
	<empleado Puesto="Cuidador" ValorDocumentoIdentidad="2161775" Nombre="Jessica Murphy" FechaContratacion="2017-04-12"/> 	
	<empleado Puesto="Fontanero" ValorDocumentoIdentidad="2918773" Nombre="Nancy Newton PhD" FechaContratacion="2016-11-22"/> 
</Empleados> 
<Usuarios> 
	<usuario Id="1" Nombre="UsuarioScripts" Pass="UsuarioScripts"/> 
	<usuario Id="1" Nombre="mgarrison" Pass=")*2LnSr^lk"/> 
	<usuario Id="2" Nombre="jgonzalez" Pass="3YSI0Hti&I"/> 
	<usuario Id="3" Nombre="zkelly" Pass="X4US4aLam@"/> 
	<usuario Id="4" Nombre="andersondeborah" Pass="732F34xo%S"/> 
	<usuario Id="5" Nombre="hardingmicheal" Pass="himB9Dzd%_"/> 
</Usuarios> 
<Movimientos> 
	<movimiento ValorDocId="7517662" IdTipoMovimiento="Venta de vacaciones" Fecha="2024-01-18" Monto="2" PostByUser="hardingmicheal" PostInIP="42.142.119.153" PostTime="2024-01-18 18:47:14"/> 
	<movimiento ValorDocId="6993943" IdTipoMovimiento="Bono vacacional" Fecha="2023-10-31" Monto="1" PostByUser="mgarrison" PostInIP="156.92.82.57" PostTime="2023-10-31 12:43:18"/> 
	<movimiento ValorDocId="8326328" IdTipoMovimiento="Venta de vacaciones" Fecha="2023-11-22" Monto="7" PostByUser="andersondeborah" PostInIP="218.213.110.232" PostTime="2023-11-22 00:23:53"/> 
	<movimiento ValorDocId="4510358" IdTipoMovimiento="Reversion de Credito" Fecha="2023-07-03" Monto="3" PostByUser="hardingmicheal" PostInIP="143.42.131.166" PostTime="2023-07-03 17:07:39"/> 
	<movimiento ValorDocId="8403646" IdTipoMovimiento="Reversion de Credito" Fecha="2023-12-07" Monto="8" PostByUser="zkelly" PostInIP="155.44.100.105" PostTime="2023-12-07 15:44:30"/> 
	<movimiento ValorDocId="8326328" IdTipoMovimiento="Venta de vacaciones" Fecha="2023-11-26" Monto="10" PostByUser="hardingmicheal" PostInIP="141.163.255.56" PostTime="2023-11-26 09:33:41"/> 
	<movimiento ValorDocId="6993943" IdTipoMovimiento="Disfrute de vacaciones" Fecha="2023-11-20" Monto="6" PostByUser="hardingmicheal" PostInIP="4.176.52.1" PostTime="2023-11-20 23:31:41"/> 
	<movimiento ValorDocId="2918773" IdTipoMovimiento="Disfrute de vacaciones" Fecha="2023-10-30" Monto="10" PostByUser="zkelly" PostInIP="220.164.108.231" PostTime="2023-10-30 03:55:57"/> 
	<movimiento ValorDocId="2161775" IdTipoMovimiento="Reversion Debito" Fecha="2023-06-13" Monto="2" PostByUser="hardingmicheal" PostInIP="135.223.57.22" PostTime="2023-06-13 13:28:39"/> 
	<movimiento ValorDocId="8403646" IdTipoMovimiento="Bono vacacional" Fecha="2024-01-01" Monto="6" PostByUser="zkelly" PostInIP="150.250.94.62" PostTime="2024-01-01 05:17:10"/> 
	<movimiento ValorDocId="2918773" IdTipoMovimiento="Venta de vacaciones" Fecha="2023-07-12" Monto="6" PostByUser="hardingmicheal" PostInIP="218.191.123.15" PostTime="2023-07-12 09:10:16"/> 
	<movimiento ValorDocId="5095109" IdTipoMovimiento="Reversion de Credito" Fecha="2023-12-27" Monto="14" PostByUser="hardingmicheal" PostInIP="136.103.23.170" PostTime="2023-12-27 12:59:03"/> 
	<movimiento ValorDocId="6993943" IdTipoMovimiento="Venta de vacaciones" Fecha="2023-04-08" Monto="1" PostByUser="jgonzalez" PostInIP="158.48.100.86" PostTime="2023-04-08 01:24:38"/> 
	<movimiento ValorDocId="8403646" IdTipoMovimiento="Bono vacacional" Fecha="2023-08-25" Monto="8" PostByUser="jgonzalez" PostInIP="204.0.219.231" PostTime="2023-08-25 16:24:07"/> 
	<movimiento ValorDocId="5095109" IdTipoMovimiento="Bono vacacional" Fecha="2024-03-07" Monto="7" PostByUser="andersondeborah" PostInIP="208.0.4.33" PostTime="2024-03-07 08:19:28"/> 
</Movimientos> 
<Error> 
	<error Codigo="50001" Descripcion="Username no existe"/> 
	<error Codigo="50002" Descripcion="Password no existe"/> 
	<error Codigo="50003" Descripcion="Login deshabilitado"/> 
	<error Codigo="50004" Descripcion="Empleado con ValorDocumentoIdentidad ya existe en inserción"/> 
	<error Codigo="50005" Descripcion="Empleado con mismo nombre ya existe en inserción"/> 
	<error Codigo="50006" Descripcion="Empleado con ValorDocumentoIdentidad ya existe en actualizacion"/> 
	<error Codigo="50007" Descripcion="Empleado con mismo nombre ya existe en actualización"/> 
	<error Codigo="50008" Descripcion="Error de base de datos"/> 
	<error Codigo="50009" Descripcion="Nombre de empleado no alfabético"/> 
	<error Codigo="50010" Descripcion="Valor de documento de identidad no alfabético"/> 
	<error Codigo="50011" Descripcion="Monto del movimiento rechazado pues si se aplicar el saldo seria negativo."/> 
</Error> 
</Datos> '''

fo = open("Datos.xml", "w")

listaSeparada = datos.split(" \\n")
fo.writelines(listaSeparada)

fo.close()
