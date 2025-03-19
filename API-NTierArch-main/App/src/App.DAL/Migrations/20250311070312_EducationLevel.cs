using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EducationLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTeacher",
                table: "Graduates");

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "Graduates",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "Graduates");

            migrationBuilder.AddColumn<bool>(
                name: "IsTeacher",
                table: "Graduates",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
