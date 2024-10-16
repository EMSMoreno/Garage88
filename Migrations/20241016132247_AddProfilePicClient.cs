using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage88.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePicClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "PhotoId",
                table: "Mechanics",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId",
                table: "Clients",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureId",
                table: "Clients");

            migrationBuilder.AlterColumn<Guid>(
                name: "PhotoId",
                table: "Mechanics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
