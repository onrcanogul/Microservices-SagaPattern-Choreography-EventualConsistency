using Microsoft.EntityFrameworkCore;

namespace Stock.API.Models.Contexts
{
    public class StockAPIDbContext : DbContext
    {
        public StockAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
    }
}
