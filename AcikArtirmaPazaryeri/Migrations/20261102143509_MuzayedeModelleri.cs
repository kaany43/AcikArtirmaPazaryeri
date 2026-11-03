using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AcikArtirmaPazaryeri.Migrations
{
    /// <inheritdoc />
    public partial class MuzayedeModelleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Markalar",
                columns: table => new
                {
                    MarkaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarkaAdi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markalar", x => x.MarkaId);
                });

            migrationBuilder.CreateTable(
                name: "Muzayedeler",
                columns: table => new
                {
                    MuzayedeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Baslik = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    KategoriId = table.Column<int>(type: "integer", nullable: false),
                    SaticiId = table.Column<string>(type: "text", nullable: false),
                    BaslangicFiyati = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    RezervFiyati = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Durum = table.Column<string>(type: "text", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: false),
                    MinimumArtis = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BaslangicZamani = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BitisZamani = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OlusturmaZamani = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UrunDurumu = table.Column<string>(type: "text", nullable: false),
                    Sehir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muzayedeler", x => x.MuzayedeId);
                    table.ForeignKey(
                        name: "FK_Muzayedeler_AspNetUsers_SaticiId",
                        column: x => x.SaticiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Muzayedeler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "KategoriId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AracModelleri",
                columns: table => new
                {
                    AracModeliId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelAdi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MarkaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AracModelleri", x => x.AracModeliId);
                    table.ForeignKey(
                        name: "FK_AracModelleri_Markalar_MarkaId",
                        column: x => x.MarkaId,
                        principalTable: "Markalar",
                        principalColumn: "MarkaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MuzayedeResimleri",
                columns: table => new
                {
                    MuzayedeResmiId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MuzayedeId = table.Column<int>(type: "integer", nullable: false),
                    DosyaYolu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Sira = table.Column<int>(type: "integer", nullable: false),
                    KapakMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuzayedeResimleri", x => x.MuzayedeResmiId);
                    table.ForeignKey(
                        name: "FK_MuzayedeResimleri_Muzayedeler_MuzayedeId",
                        column: x => x.MuzayedeId,
                        principalTable: "Muzayedeler",
                        principalColumn: "MuzayedeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AracDetaylari",
                columns: table => new
                {
                    AracDetaylariId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MuzayedeId = table.Column<int>(type: "integer", nullable: false),
                    SasiNo = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    Plaka = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AracModeliId = table.Column<int>(type: "integer", nullable: false),
                    Yil = table.Column<int>(type: "integer", nullable: false),
                    Km = table.Column<int>(type: "integer", nullable: false),
                    Renk = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    YakitTuru = table.Column<string>(type: "text", nullable: false),
                    VitesTuru = table.Column<string>(type: "text", nullable: false),
                    KasaTipi = table.Column<string>(type: "text", nullable: false),
                    Paket = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AracDetaylari", x => x.AracDetaylariId);
                    table.ForeignKey(
                        name: "FK_AracDetaylari_AracModelleri_AracModeliId",
                        column: x => x.AracModeliId,
                        principalTable: "AracModelleri",
                        principalColumn: "AracModeliId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AracDetaylari_Muzayedeler_MuzayedeId",
                        column: x => x.MuzayedeId,
                        principalTable: "Muzayedeler",
                        principalColumn: "MuzayedeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EkspertizKalemleri",
                columns: table => new
                {
                    EkspertizKalemiId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AracDetaylariId = table.Column<int>(type: "integer", nullable: false),
                    Parca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Durum = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EkspertizKalemleri", x => x.EkspertizKalemiId);
                    table.ForeignKey(
                        name: "FK_EkspertizKalemleri_AracDetaylari_AracDetaylariId",
                        column: x => x.AracDetaylariId,
                        principalTable: "AracDetaylari",
                        principalColumn: "AracDetaylariId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AracDetaylari_AracModeliId",
                table: "AracDetaylari",
                column: "AracModeliId");

            migrationBuilder.CreateIndex(
                name: "IX_AracDetaylari_MuzayedeId",
                table: "AracDetaylari",
                column: "MuzayedeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AracModelleri_MarkaId",
                table: "AracModelleri",
                column: "MarkaId");

            migrationBuilder.CreateIndex(
                name: "IX_EkspertizKalemleri_AracDetaylariId",
                table: "EkspertizKalemleri",
                column: "AracDetaylariId");

            migrationBuilder.CreateIndex(
                name: "IX_Muzayedeler_KategoriId",
                table: "Muzayedeler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Muzayedeler_SaticiId",
                table: "Muzayedeler",
                column: "SaticiId");

            migrationBuilder.CreateIndex(
                name: "IX_MuzayedeResimleri_MuzayedeId",
                table: "MuzayedeResimleri",
                column: "MuzayedeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EkspertizKalemleri");

            migrationBuilder.DropTable(
                name: "MuzayedeResimleri");

            migrationBuilder.DropTable(
                name: "AracDetaylari");

            migrationBuilder.DropTable(
                name: "AracModelleri");

            migrationBuilder.DropTable(
                name: "Muzayedeler");

            migrationBuilder.DropTable(
                name: "Markalar");
        }
    }
}
