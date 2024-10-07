using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Tarea2_BD1.Models
{
    public class ValidacionesEstaticas
    {

        [HttpPost]
        public static string ConsultaCodError(string codigo, Dbtarea2Context dbContext)
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_ConsultaError";

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
                comando.Parameters.Add(paramCodigo);
                comando.Parameters.Add(paramResultado);

                //Se leen los datos devueltos por el SP(dataset)
                SqlDataReader reader = comando.ExecuteReader();
                reader.Read();
                string descripcionError = reader.GetString(0);
                reader.Close();

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_ConsultaError -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                return descripcionError;
            }
            catch (Exception ex)
            {
                return String.Format("El error es: {0}", ex.ToString());
            }
        }
    }
}
