using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrator.Migrations
{
    public partial class _002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Column2",
                table: "FooTable",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Column2",
                table: "FooTable");
        }
    }
}
