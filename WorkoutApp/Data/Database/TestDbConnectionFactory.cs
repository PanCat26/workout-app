using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutApp.Data.Database
{
    class TestDbConnectionFactory: DbConnectionFactory
    {
        public TestDbConnectionFactory(string connectionString)
        : base(connectionString)
        {
        }

        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
