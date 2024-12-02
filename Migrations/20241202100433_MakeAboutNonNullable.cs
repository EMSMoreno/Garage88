using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage88.Migrations
{
    /// <inheritdoc />
    public partial class MakeAboutNonNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Mechanics SET About = 'No details available.' WHERE About IS NULL;");

            // Alterações das colunas RoleId e SpecialityId
            migrationBuilder.AlterColumn<int>(
                name: "SpecialityId",
                table: "Mechanics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Mechanics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Torna a coluna About não nula
            migrationBuilder.AlterColumn<string>(
                name: "About",
                table: "Mechanics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
            name: "SpecialityId",
            table: "Mechanics",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Mechanics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "About",
                table: "Mechanics",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
