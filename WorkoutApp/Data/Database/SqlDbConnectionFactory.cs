using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WorkoutApp.Data.Database
{
    public class SqlDbConnectionFactory: DbConnectionFactory
    {
        public SqlDbConnectionFactory(string connectionString)
        : base(connectionString)
        {
        }

        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
