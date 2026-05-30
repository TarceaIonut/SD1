using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Specialty",
                table: "Persons",
                newName: "specialty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "specialty",
                table: "Persons",
                newName: "Specialty");
        }
    }
}
