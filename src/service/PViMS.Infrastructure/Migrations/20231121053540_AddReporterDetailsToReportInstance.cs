using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PViMS.Infrastructure.Migrations
{
    public partial class AddReporterDetailsToReportInstance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReporterEmail",
                table: "ReportInstance",
                type: "LONGTEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterFullName",
                table: "ReportInstance",
                type: "LONGTEXT",
                nullable: true);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "System",
            //    table: "DatasetCategoryElement",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Public",
            //    table: "DatasetCategoryElement",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Chronic",
            //    table: "DatasetCategoryElement",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Acute",
            //    table: "DatasetCategoryElement",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "System",
            //    table: "DatasetCategory",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Public",
            //    table: "DatasetCategory",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Chronic",
            //    table: "DatasetCategory",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Acute",
            //    table: "DatasetCategory",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReporterEmail",
                table: "ReportInstance");

            migrationBuilder.DropColumn(
                name: "ReporterFullName",
                table: "ReportInstance");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "System",
            //    table: "DatasetCategoryElement",
            //    type: "bit",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldDefaultValue: false);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Public",
            //    table: "DatasetCategoryElement",
            //    type: "bit",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldDefaultValue: false);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Chronic",
            //    table: "DatasetCategoryElement",
            //    type: "bit",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldDefaultValue: false);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Acute",
            //    table: "DatasetCategoryElement",
            //    type: "bit",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldDefaultValue: false);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "System",
            //    table: "DatasetCategory",
            //    type: "bit",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldDefaultValue: false);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Public",
            //    table: "DatasetCategory",
            //    type: "bit",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldDefaultValue: false);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Chronic",
            //    table: "DatasetCategory",
            //    type: "bit",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldDefaultValue: false);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Acute",
            //    table: "DatasetCategory",
            //    type: "bit",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldDefaultValue: false);
        }
    }
}
