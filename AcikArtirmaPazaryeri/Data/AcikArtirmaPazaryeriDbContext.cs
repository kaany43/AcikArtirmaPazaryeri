using AcikArtirmaPazaryeri.Models;
using Microsoft.EntityFrameworkCore;

namespace AcikArtirmaPazaryeri.Data
{
    public class AcikArtirmaPazaryeriDbContext : DbContext
    {
        public AcikArtirmaPazaryeriDbContext(DbContextOptions<AcikArtirmaPazaryeriDbContext> options)
            : base(options)
        {
        }
        public DbSet<Kategori> Kategoriler { get; set; }
    }
}