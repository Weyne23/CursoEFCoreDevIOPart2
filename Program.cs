using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //FiltroGlobal();
            //IgnoreFiltroGlobal();
            //ConsultaProjetada();
            //ConsultaParametrizada();
            //ConsultaInterpolada();
            //ConsultaComTag();
            //EntendendoConsulta1NN1();
            DivisaoDeConsultas();
        }
        
        static void DivisaoDeConsultas()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .Include(x => x.Funcionarios)
                .Where(x => x.Id < 3)
                //.AsSplitQuery() Feito de forma global, olhar o Context
                .AsSingleQuery() //Para não executar o Split Query
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\tNome: {funcionario.Nome}");
                }
            }
        }

        static void EntendendoConsulta1NN1()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            // var departamentos = db.Departamentos
            //     .Include(x => x.Funcionarios)
            //     .ToList();

            var funcionarios = db.Funcionarios
                 .Include(x => x.Departamento)
                 .ToList();

            foreach (var funcionario in funcionarios)
            {
                Console.WriteLine($"Funcionario: {funcionario.Nome} Descrição: {funcionario.Departamento.Descricao}");
            }
        }

        static void ConsultaComTag()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .TagWith(@"Estou enviando um comentario para o servidor.
                Segundo Comentario
                Terceiro Comentario") //@ serve para colocar mais de 1 comentario
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void ConsultaInterpolada()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var id = 1;

            //var id = 0;//Tambem funciona, entity faz a de cima autom atico

            var departamentos = db.Departamentos
                .FromSqlInterpolated($"SELECT * FROM Departamentos WHERE Id > {id}")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void ConsultaParametrizada()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var id = new SqlParameter
            {
                Value = 1,
                SqlDbType = System.Data.SqlDbType.Int
            };

            //var id = 0;//Tambem funciona, entity faz a de cima autom atico

            var departamentos = db.Departamentos
                .FromSqlRaw("SELECT * FROM Departamentos WHERE Id > {0}", id)
                .Where(p => !p.Excluido)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void ConsultaProjetada()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
            .Where(x => x.Id > 0)
            .Select(p => new { p.Descricao, Funcionarios = p.Funcionarios.Select(f => f.Nome)})
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Nome: {funcionario}");
                }
            }
        }
        static void IgnoreFiltroGlobal()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(x => x.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \t Excluido: {departamento.Excluido}");
            }
        }

        static void FiltroGlobal()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.Where(x => x.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \t Excluido: {departamento.Excluido}");
            }
        }

        static void Setup(Curso.Data.ApplicationContext db)
        {
            if(db.Database.EnsureCreated())
            {
                db.Departamentos.AddRange(
                    new Curso.Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Rafael Almeida",
                                CPF = "99999999911",
                                RG = "2100062"
                            }
                        },
                        Excluido = true
                    },
                    new Curso.Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Bruno Brito",
                                CPF = "99988999911",
                                RG = "3100062"
                            },
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Eduardo Pires",
                                CPF = "99779999911",
                                RG = "4100062"
                            }
                        }
                    }
                );
            }

            db.SaveChanges();
            db.ChangeTracker.Clear();
        }
    }
}
