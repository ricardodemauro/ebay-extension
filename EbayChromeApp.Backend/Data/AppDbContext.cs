using EbayChromeApp.Backend.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Search> Search { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Search>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Search>()
                .HasIndex(x => x.IP);
            modelBuilder.Entity<Search>()
                .HasIndex(x => x.Created);

            base.OnModelCreating(modelBuilder);
        }
    }
}
