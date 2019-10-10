using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrator.Migrations
{
    public partial class _001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Column1",
                table: "FooTable",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Column1",
                table: "FooTable");
        }
    }
}
