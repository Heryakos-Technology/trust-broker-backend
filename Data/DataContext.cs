

using System;
using broker.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata; // Add this line

namespace broker.Data

{
    public class DataContext : DbContext
    {
        // public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }




        public DbSet<User> Users { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<City> Cities { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<Saving> Savings { get; set; }
        public DbSet<Deals> Deals { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Buy> Buys { get; set; }
        public DbSet<Sales> Sales { get; set; }

         protected override void OnModelCreating(ModelBuilder builder)  
        {  
        builder.Entity<Customer>()
            .Property(c => c.CustomerId)
            .ValueGeneratedOnAdd(); 
        }  
  
        public override int SaveChanges()  
        {  
            ChangeTracker.DetectChanges();  
            return base.SaveChanges();  
        }
    }
}














//     }
// }

