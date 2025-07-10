using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        var connStr = "Server=LAPTOP-2VN5LPKO\\SQLEXPRESS;Database=DENTAL360;Trusted_Connection=True;TrustServerCertificate=True;";
        using (var conn = new SqlConnection(connStr))
        {
            try
            {
                conn.Open();
                Console.WriteLine("Conexão OK!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }
    }
}
