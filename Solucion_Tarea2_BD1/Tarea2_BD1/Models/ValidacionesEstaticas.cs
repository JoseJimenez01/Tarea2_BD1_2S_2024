using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Tarea2_BD1.Models
{
    public class ValidacionesEstaticas
    {

        [HttpPost]
        public static async Task<string> ConsultaCodError(string codigo, Dbtarea2Context dbContext)
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_ConsultaError";

                //Código para crear parámetros al Store Procedure
                //SqlParameter paramUsername = new SqlParameter
                //{
                //    ParameterName = "@inUsername",
                //    SqlDbType = SqlDbType.VarChar,
                //    Size = 64,
                //    Value = usuario,
                //    Direction = ParameterDirection.Input
                //};

                SqlParameter paramCodigo = new SqlParameter
                {
                    ParameterName = "@inCodigo",
                    SqlDbType = SqlDbType.Int,
                    Value = int.Parse(codigo), //lo cambiamos a int, porque asi esta guardado en la base de datos
                    Direction = ParameterDirection.Input
                };

                SqlParameter paramResultado = new SqlParameter
                {
                    ParameterName = "@outResult",
                    SqlDbType = SqlDbType.Int,
                    Value = -345678,
                    Direction = ParameterDirection.InputOutput
                };

                //Se agrega cada parámetro al SP
                //comando.Parameters.Add(paramUsername);
                comando.Parameters.Add(paramCodigo);
                comando.Parameters.Add(paramResultado);

                //Se leen los datos devueltos por el SP(dataset)
                SqlDataReader reader = await comando.ExecuteReaderAsync();
                await reader.ReadAsync();
                string descripcionError = reader.GetString(0);
                await reader.CloseAsync();

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_ConsultaError -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return descripcionError;
            }
            catch (Exception ex)
            {
                return String.Format("El error es: {0}", ex.ToString());
            }
        }
    }
}
