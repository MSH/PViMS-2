using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PViMS.Infrastructure.Migrations
{
    public partial class AddNameToDashboardSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DashboardSeries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "DashboardSeries");
        }
    }
}
