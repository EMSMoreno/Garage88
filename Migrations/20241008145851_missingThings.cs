using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage88.Migrations
{
    /// <inheritdoc />
    public partial class missingThings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mechanics_Specialties_SpecialtyId",
                table: "Mechanics");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialties_MechanicsRoles_RoleId",
                table: "Specialties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialties",
                table: "Specialties");

            migrationBuilder.RenameTable(
                name: "Specialties",
                newName: "Specialities");

            migrationBuilder.RenameColumn(
                name: "SpecialtyId",
                table: "Mechanics",
                newName: "SpecialityId");

            migrationBuilder.RenameIndex(
                name: "IX_Mechanics_SpecialtyId",
                table: "Mechanics",
                newName: "IX_Mechanics_SpecialityId");

            migrationBuilder.RenameIndex(
                name: "IX_Specialties_RoleId",
                table: "Specialities",
                newName: "IX_Specialities_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialities",
                table: "Specialities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mechanics_Specialities_SpecialityId",
                table: "Mechanics",
                column: "SpecialityId",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Specialities_MechanicsRoles_RoleId",
                table: "Specialities",
                column: "RoleId",
                principalTable: "MechanicsRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mechanics_Specialities_SpecialityId",
                table: "Mechanics");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialities_MechanicsRoles_RoleId",
                table: "Specialities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialities",
                table: "Specialities");

            migrationBuilder.RenameTable(
                name: "Specialities",
                newName: "Specialties");

            migrationBuilder.RenameColumn(
                name: "SpecialityId",
                table: "Mechanics",
                newName: "SpecialtyId");

            migrationBuilder.RenameIndex(
                name: "IX_Mechanics_SpecialityId",
                table: "Mechanics",
                newName: "IX_Mechanics_SpecialtyId");

            migrationBuilder.RenameIndex(
                name: "IX_Specialities_RoleId",
                table: "Specialties",
                newName: "IX_Specialties_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialties",
                table: "Specialties",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mechanics_Specialties_SpecialtyId",
                table: "Mechanics",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Specialties_MechanicsRoles_RoleId",
                table: "Specialties",
                column: "RoleId",
                principalTable: "MechanicsRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
