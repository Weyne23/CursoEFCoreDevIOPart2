using System;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext: DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConection = "Data source=(localdb)\\mssqllocaldb;Initial Catalog=DevIO-02;Integrated Security=true;pooling=true"; //MultipleActiveResultSets=true
            //optionsBuilder.UseSqlServer(strConection, p => p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            optionsBuilder.UseSqlServer(strConection)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Departamento>()
                        //.HasQueryFilter(p => !p.Excluido);
        }
    }
}