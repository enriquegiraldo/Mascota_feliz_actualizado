using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MascotaFeliz.App.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class AddProximaVacuna : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ProximaVacuna",
                table: "VisitasPyP",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoProximaVacuna",
                table: "VisitasPyP",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProximaVacuna",
                table: "VisitasPyP");

            migrationBuilder.DropColumn(
                name: "TipoProximaVacuna",
                table: "VisitasPyP");
        }
    }
}
