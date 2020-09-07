using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityResolution
{
    public class Program
    {
        private static void Main(string[] args)
        {
            using var db = new IdentityResolutionContext();
            CargaInicial(db);

            var produtos = db
                .Produtos
                    .Include(p => p.Vendedor)
                //.AsNoTrackingWithIdentityResolution()
                .AsNoTracking()
                .ToList();
        }

        private static void CargaInicial(IdentityResolutionContext db)
        {
            if (db.Database.EnsureCreated())
            {
                db.Vendedores.AddRange(Enumerable.Range(1, 100).Select(v =>
                new Vendedor
                {
                    Nome = $"Vendedor - {v}",
                    Produtos = Enumerable.Range(1, 100).Select(p =>
                        new Produto
                        {
                            Descricao = $"Produto {p}, Vendedor {v}",
                            Valor = p
                        }).ToList()
                }));

                db.SaveChanges();
            } 
            
        }
    }


public class IdentityResolutionContext : DbContext
{
    public DbSet<Vendedor> Vendedores { get; set; }
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            .UseSqlServer(
                "Data source=(localdb)\\mssqllocaldb;Initial Catalog=IdentityResolution5;Integrated Security=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vendedor>(p =>
        {
            p.ToTable("Vendedores");
            p.HasMany<Produto>()
                .WithOne(p => p.Vendedor);
        });
    }
}

    public class Vendedor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public IList<Produto> Produtos { get; set; }
    }

    public class Produto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public Vendedor Vendedor { get; set; }
    }
}
