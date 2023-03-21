using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PViMS.Infrastructure.Migrations
{
    public partial class RemoveLegacyFieldDashboardSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DashboardDatasetElementId",
                table: "DashboardSeries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DashboardDatasetElementId",
                table: "DashboardSeries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
