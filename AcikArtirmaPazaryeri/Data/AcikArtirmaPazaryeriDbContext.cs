using AcikArtirmaPazaryeri.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace AcikArtirmaPazaryeri.Data
{
    public class AcikArtirmaPazaryeriDbContext : IdentityDbContext<Kullanici>
    {
        public AcikArtirmaPazaryeriDbContext(DbContextOptions<AcikArtirmaPazaryeriDbContext> options)
            : base(options)
        {
        }
        public DbSet<Kategori> Kategoriler { get; set; }
    }
}