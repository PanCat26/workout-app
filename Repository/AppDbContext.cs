using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Models;

namespace WorkoutApp.Repository
{
    public class AppDbContext : DbContext
    {
        public DbSet<MockModel> Mock{ get; set; }

        /// <summary>
        /// Pentru migrare trebuie sa pui connection string-u sa ai si Modelele pregatite in cod, dupa care in consola de package manager alegi proiectul cu contextul si scrii "Add-Migration InitialCreate" si apoi "Update-Database" sau "dotnet ef migrations add InitialCreate" si "dotnet ef database update"
        /// </summary>
        /// <param name="options"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=WinUiSqlAppDb;Trusted_Connection=True;"); //Aici trebuie connection string-ul
        }



    }
}
