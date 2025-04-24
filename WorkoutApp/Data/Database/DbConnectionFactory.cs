using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutApp.Data.Database
{
    public abstract class DbConnectionFactory
    {
        protected readonly string _connectionString;
        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public abstract IDbConnection CreateConnection();
    }
}
