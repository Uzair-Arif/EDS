using EDS.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Infrastructure.Data
{
    public class AppDbContext: IdentityDbContext
    {

        public DbSet<Member> Members { get; set; }

        public DbSet<Friend> Friends { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-9340E4V;Database=CLDB;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           

            modelBuilder.Entity<Friend>()
    .HasKey(f => new { f.Member1Id, f.Member2Id });

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Member1)
                .WithMany()
                .HasForeignKey(f => f.Member1Id).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Member2)
                .WithMany()
                .HasForeignKey(f => f.Member2Id);

            //Seed Data Department
            //modelBuilder.Entity<Department>().HasData(
            //    new Department
            //    {
            //         Id=1,
            //         Name ="HR"
            //    });
            //modelBuilder.Entity<Department>().HasData(
            //    new Department
            //    {
            //        Id = 2,
            //        Name = "Business"
            //    });
            //modelBuilder.Entity<Department>().HasData(
            //    new Department
            //    {
            //        Id = 3,
            //        Name = "Development"
            //    });


            ////Seed Employee Data
            //modelBuilder.Entity<Employee>().HasData( 
            //    new Employee 
            //    {  
            //      Id=1,
            //      Name="Alex",
            //       DepartmentId=1,
            //      Email="Alex@gmail.com",    
            //       Designation="Assistant HR"
            //    });

            //modelBuilder.Entity<Employee>().HasData(
            //    new Employee
            //    {
            //        Id = 2,
            //        Name = "Josh",
            //        DepartmentId = 2,
            //        Email = "josh@gmail.com",
            //        Designation = "Business Analyst"
            //    });
            //modelBuilder.Entity<Employee>().HasData(
            //    new Employee
            //    {
            //        Id = 3,
            //        Name = "Yuan",
            //        DepartmentId = 3,
            //        Email = "yuan@gmail.com",
            //        Designation = "Software Engineer"
            //    });

        }
    }
}
