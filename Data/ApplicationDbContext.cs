using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shiftbid.Models;

namespace Shiftbid.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Seniority> Seniorities { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder
            .Entity<Report>()
            .Property(r => r.Status)
            .HasConversion(
                v => v.ToString(),
                v => (Status)Enum.Parse(typeof(Status), v)
            );
        }
    }
}
