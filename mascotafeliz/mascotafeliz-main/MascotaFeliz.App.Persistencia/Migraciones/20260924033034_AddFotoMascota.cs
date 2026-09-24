using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MascotaFeliz.App.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class AddFotoMascota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Mascotas",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Mascotas");
        }
    }
}
