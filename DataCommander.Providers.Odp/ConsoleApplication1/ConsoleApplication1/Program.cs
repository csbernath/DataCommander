using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnectionStringBuilder cs = new SqlConnectionStringBuilder();
            cs.DataSource = "CAMSCOBUSQL2\S2";
            cs.InitialCatalog = "EzYBankingSK";
            cs.IntegratedSecurity = true;
            cs.
            string connectionString = cs.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataContext dataContext = new DataContext(connection);


                dataContext.GetTable<object>().g
            }
        }
    }
}
