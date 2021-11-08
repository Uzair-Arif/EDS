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

        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
         : base(options)
        { }

        public DbSet<Member> Members { get; set; }

        public DbSet<Friend> Friends { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=DESKTOP-9340E4V;Database=CLDB;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           

            modelBuilder.Entity<Friend>()
            .HasKey(f => new { f.Member1Id, f.Member2Id });

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Member1)
                .WithMany(f=>f.MemberFriends)
                .HasForeignKey(f => f.Member1Id).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Member2)
                .WithMany(f=>f.MemberFriendsOf)
                .HasForeignKey(f => f.Member2Id);


        }
    }
}
