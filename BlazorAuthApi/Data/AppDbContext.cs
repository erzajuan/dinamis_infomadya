
using Microsoft.EntityFrameworkCore;
using BlazorAuthApi.Models;

namespace BlazorAuthApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }

}