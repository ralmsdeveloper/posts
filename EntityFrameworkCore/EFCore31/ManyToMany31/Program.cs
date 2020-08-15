using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ManyToMany31
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

            var rows = db.SaveChanges();
            var students = db.Students.AsNoTracking().Include(p => p.CourseStudents).ToList();

            Console.WriteLine("Hello World!");
        }
    }


    public class SampleManyToManyContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseStudent>()
                .HasKey(p => new { p.CourseId, p.StudentId });

            modelBuilder.Entity<CourseStudent>()
                .HasOne(p => p.Student)
                .WithMany(p => p.CourseStudents)
                .HasForeignKey(p => p.StudentId);

            modelBuilder.Entity<CourseStudent>()
                .HasOne(p => p.Course)
                .WithMany(p => p.CourseStudents)
                .HasForeignKey(p => p.CourseId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer("Data source=(localdb)\\mssqllocaldb;Initial Catalog=SampleManyToMany31;Integrated Security=true");
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<CourseStudent> CourseStudents { get; } = new List<CourseStudent>();
    }

    public class Course
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public IList<CourseStudent> CourseStudents { get; } = new List<CourseStudent>();
    }

    public class CourseStudent
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
