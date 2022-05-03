using Microsoft.EntityFrameworkCore;
using DemoAPI2.Models;
namespace DemoAPI2.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> option): base(option)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
