using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PViMS.Infrastructure.Migrations
{
    public partial class AddDashboardAggregate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "System",
                table: "DatasetCategoryElement",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Public",
                table: "DatasetCategoryElement",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Chronic",
                table: "DatasetCategoryElement",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Acute",
                table: "DatasetCategoryElement",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "System",
                table: "DatasetCategory",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Public",
                table: "DatasetCategory",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Chronic",
                table: "DatasetCategory",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Acute",
                table: "DatasetCategory",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "Dashboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LongName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrequencyId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dashboard_User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dashboard_User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DashboardElement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LongName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    SourceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DashboardId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardElement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardElement_Dashboard_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DashboardElement_User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DashboardElement_User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DashboardSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Attributes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DashboardDatasetElementId = table.Column<int>(type: "int", nullable: false),
                    ListSQL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueSQL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DashboardUnitId = table.Column<int>(type: "int", nullable: false),
                    DashboardElementId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardSeries_DashboardElement_DashboardElementId",
                        column: x => x.DashboardElementId,
                        principalTable: "DashboardElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DashboardSeries_User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DashboardSeries_User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DashboardVisualisation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChartTypeId = table.Column<int>(type: "int", nullable: false),
                    Attributes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DashboardElementId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardVisualisation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardVisualisation_DashboardElement_DashboardElementId",
                        column: x => x.DashboardElementId,
                        principalTable: "DashboardElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DashboardVisualisation_User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DashboardVisualisation_User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_CreatedBy_Id",
                table: "Dashboard",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_UpdatedBy_Id",
                table: "Dashboard",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardElement_CreatedBy_Id",
                table: "DashboardElement",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardElement_DashboardId",
                table: "DashboardElement",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardElement_UpdatedBy_Id",
                table: "DashboardElement",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSeries_CreatedBy_Id",
                table: "DashboardSeries",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSeries_DashboardElementId",
                table: "DashboardSeries",
                column: "DashboardElementId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSeries_UpdatedBy_Id",
                table: "DashboardSeries",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardVisualisation_CreatedBy_Id",
                table: "DashboardVisualisation",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardVisualisation_DashboardElementId",
                table: "DashboardVisualisation",
                column: "DashboardElementId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardVisualisation_UpdatedBy_Id",
                table: "DashboardVisualisation",
                column: "UpdatedBy_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardSeries");

            migrationBuilder.DropTable(
                name: "DashboardVisualisation");

            migrationBuilder.DropTable(
                name: "DashboardElement");

            migrationBuilder.DropTable(
                name: "Dashboard");

            migrationBuilder.AlterColumn<bool>(
                name: "System",
                table: "DatasetCategoryElement",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Public",
                table: "DatasetCategoryElement",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Chronic",
                table: "DatasetCategoryElement",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Acute",
                table: "DatasetCategoryElement",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "System",
                table: "DatasetCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Public",
                table: "DatasetCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Chronic",
                table: "DatasetCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Acute",
                table: "DatasetCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }
    }
}
