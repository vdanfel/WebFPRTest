using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using WebFPRTest.Interface;
using WebFPRTest.Models;
using WebFPRTest.Result;

namespace WebFPRTest.Service
{
    public class LoginService:ILoginService
    {
        private readonly SqlConnection _connection;
        public LoginService(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<LoginResult> ValidarLogin(LoginViewModel login)
        {
            const string storedProcedure = "usp_Login_Validar";
            try
            {
                string claveEncriptada = EncriptarClave(login.Clave);
                var usuario = await _connection.QuerySingleOrDefaultAsync<LoginResult>(
                    storedProcedure,
                    new
                    {
                        Usuario = login.Usuario,
                        ClaveHash = claveEncriptada
                    },
                    commandType: CommandType.StoredProcedure);

                return usuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar login: {ex.Message}");
                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
        private string EncriptarClave(string password)
        {
            if (password == null)
            {
                return password;
            }
            else
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    // Retorna la clave en formato hexadecimal (encriptada)
                    return sBuilder.ToString();
                }
            }
        }
    }
}
