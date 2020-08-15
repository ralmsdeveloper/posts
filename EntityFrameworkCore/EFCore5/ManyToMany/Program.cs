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

            var student1 = new Student { Name = "Ralms" };
            var student2 = new Student { Name = "Joe" };
            var course1 = new Course { Description = "Developer" };

            student1.Courses.Add(course1);
            course1.Students.Add(student1);

            db.Add(student1);
            db.Add(student2);
            db.Add(course1); 

            var rows = db.SaveChanges();

            var students = db.Students.AsNoTracking().Include(p => p.Courses).ToList(); 
        }
    }


    public class SampleManyToManyContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Course { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .EnableSensitiveDataLogging() // Show Data
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlServer("Data source=(localdb)\\mssqllocaldb;Initial Catalog=SampleManyToMany5;Integrated Security=true");
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
}
