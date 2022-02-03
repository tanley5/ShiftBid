using Microsoft.EntityFrameworkCore.Migrations;

namespace Shiftbid.Data.Migrations
{
    public partial class SeniorityState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeniorityState",
                table: "Seniorities",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeniorityState",
                table: "Seniorities");
        }
    }
}
