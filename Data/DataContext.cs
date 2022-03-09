using GroceryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryManagement.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Product>().HasOne(p => p.Category).WithOne()
        //}
    }
}
