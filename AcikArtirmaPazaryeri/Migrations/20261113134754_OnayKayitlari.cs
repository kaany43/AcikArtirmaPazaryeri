using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AcikArtirmaPazaryeri.Migrations
{
    /// <inheritdoc />
    public partial class OnayKayitlari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnayKayitlari",
                columns: table => new
                {
                    OnayKaydiId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MuzayedeId = table.Column<int>(type: "integer", nullable: false),
                    Sonuc = table.Column<string>(type: "text", nullable: false),
                    Gerekce = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OtomatikMi = table.Column<bool>(type: "boolean", nullable: false),
                    YoneticiId = table.Column<string>(type: "text", nullable: true),
                    Tarih = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnayKayitlari", x => x.OnayKaydiId);
                    table.ForeignKey(
                        name: "FK_OnayKayitlari_AspNetUsers_YoneticiId",
                        column: x => x.YoneticiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnayKayitlari_Muzayedeler_MuzayedeId",
                        column: x => x.MuzayedeId,
                        principalTable: "Muzayedeler",
                        principalColumn: "MuzayedeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnayKayitlari_MuzayedeId",
                table: "OnayKayitlari",
                column: "MuzayedeId");

            migrationBuilder.CreateIndex(
                name: "IX_OnayKayitlari_YoneticiId",
                table: "OnayKayitlari",
                column: "YoneticiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnayKayitlari");
        }
    }
}
