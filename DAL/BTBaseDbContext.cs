using System;
using AppListWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AppListWebAPI.DAL
{
    public class BTBaseDbContext : DbContext
    {
        public virtual DbSet<BTAppLaunchRecord> BTAppLaunchRecord { get; set; }

        public BTBaseDbContext(DbContextOptions<BTBaseDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            AppListWebAPI.Models.BTAppLaunchRecord.OnDbContextModelCreating(modelBuilder);
        }
    }
}