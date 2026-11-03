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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Muzayede>()
                .HasOne(m => m.AracDetaylari)
                .WithOne(a => a.Muzayede)
                .HasForeignKey<AracDetaylari>(a => a.MuzayedeId);

            builder.Entity<Muzayede>()
                .HasOne(m => m.Kategori)
                .WithMany().HasForeignKey(m => m.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Muzayede>()
                .HasOne(m => m.Satici)
                .WithMany().HasForeignKey(m => m.SaticiId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AracDetaylari>()
                .HasOne(a => a.AracModeli)
                .WithMany().HasForeignKey(a => a.AracModeliId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AracModeli>()
                .HasOne(m => m.Marka)
                .WithMany(k => k.Modeller).HasForeignKey(m => m.MarkaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder b)
        {
            b.Properties<decimal>().HavePrecision(18, 2);
            b.Properties<Enum>().HaveConversion<string>();
        }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Marka> Markalar { get; set; }
        public DbSet<AracModeli> AracModelleri { get; set; }
        public DbSet<AracDetaylari> AracDetaylari { get; set; }
        public DbSet<EkspertizKalemi> EkspertizKalemleri { get; set; }
        public DbSet<Muzayede> Muzayedeler { get; set; }
        public DbSet<MuzayedeResmi> MuzayedeResimleri { get; set; }
    }
}