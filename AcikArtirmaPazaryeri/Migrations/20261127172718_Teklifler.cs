using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AcikArtirmaPazaryeri.Migrations
{
    /// <inheritdoc />
    public partial class Teklifler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GuncelFiyat",
                table: "Muzayedeler",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Muzayedeler",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateTable(
                name: "Teklifler",
                columns: table => new
                {
                    TeklifId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MuzayedeId = table.Column<int>(type: "integer", nullable: false),
                    KullaniciId = table.Column<string>(type: "text", nullable: false),
                    Tutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Zaman = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teklifler", x => x.TeklifId);
                    table.ForeignKey(
                        name: "FK_Teklifler_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teklifler_Muzayedeler_MuzayedeId",
                        column: x => x.MuzayedeId,
                        principalTable: "Muzayedeler",
                        principalColumn: "MuzayedeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teklifler_KullaniciId",
                table: "Teklifler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Teklifler_MuzayedeId",
                table: "Teklifler",
                column: "MuzayedeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Teklifler");

            migrationBuilder.DropColumn(
                name: "GuncelFiyat",
                table: "Muzayedeler");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Muzayedeler");
        }
    }
}
