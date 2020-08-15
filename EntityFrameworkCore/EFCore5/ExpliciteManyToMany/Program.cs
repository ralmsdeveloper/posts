using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ManyToMany
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new SampleManyToManyContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var students = db.Students.Include(p => p.Courses).ToList();
            var courses = db.Courses.Include(p => p.Students).ToList();
            var courseStudent = db.Set<CourseStudent>().FirstOrDefault();

            var protocol = courseStudent.Protocol;
            var createdAt = courseStudent.CreatedAt;
        }
    }


    public class SampleManyToManyContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Student>()
                .HasMany(p => p.Courses)
                .WithMany(p => p.Students)
                .UsingEntity<CourseStudent>(
                    p => p.HasOne<Course>().WithMany(),
                    p => p.HasOne<Student>().WithMany());

            modelBuilder
                .Entity<CourseStudent>(p =>
                {
                    p.Property(e => e.Protocol).HasColumnType("VARCHAR(32)");
                    p.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .EnableSensitiveDataLogging() // Show Data
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlServer("Data source=(localdb)\\mssqllocaldb;Initial Catalog=SampleManyToManyExplicite5;Integrated Security=true");
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Course> Courses { get; } = new List<Course>();
    }

    public class Course
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public IList<Student> Students { get; } = new List<Student>();
    }

    public class CourseStudent
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public string Protocol { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
