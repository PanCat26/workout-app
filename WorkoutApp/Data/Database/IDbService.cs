using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutApp.Data.Database
{
    interface IDbService
    {
        Task<DataTable> ExecuteSelectAsync(string query, List<SqlParameter> parameters);
        Task<int> ExecuteQueryAsync(string query, List<SqlParameter> parameters);
    }
}
