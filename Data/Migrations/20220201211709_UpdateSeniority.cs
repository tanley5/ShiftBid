using Microsoft.EntityFrameworkCore.Migrations;

namespace Shiftbid.Data.Migrations
{
    public partial class UpdateSeniority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeniorityNumber",
                table: "Seniorities",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeniorityNumber",
                table: "Seniorities");
        }
    }
}
