using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_HRIS.Migrations
{
    public partial class dataanotate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayslipNumber",
                table: "TblPayslipModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreakInAm",
                table: "tbl_TimeLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreakInPm",
                table: "tbl_TimeLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreakOutAm",
                table: "tbl_TimeLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreakOutPm",
                table: "tbl_TimeLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LunchIn",
                table: "tbl_TimeLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LunchOut",
                table: "tbl_TimeLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBreakAmHours",
                table: "tbl_TimeLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBreakPmHours",
                table: "tbl_TimeLogs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalLunchHours",
                table: "tbl_TimeLogs",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayslipNumber",
                table: "TblPayslipModel");

            migrationBuilder.DropColumn(
                name: "BreakInAm",
                table: "tbl_TimeLogs");

            migrationBuilder.DropColumn(
                name: "BreakInPm",
                table: "tbl_TimeLogs");

            migrationBuilder.DropColumn(
                name: "BreakOutAm",
                table: "tbl_TimeLogs");

            migrationBuilder.DropColumn(
                name: "BreakOutPm",
                table: "tbl_TimeLogs");

            migrationBuilder.DropColumn(
                name: "LunchIn",
                table: "tbl_TimeLogs");

            migrationBuilder.DropColumn(
                name: "LunchOut",
                table: "tbl_TimeLogs");

            migrationBuilder.DropColumn(
                name: "TotalBreakAmHours",
                table: "tbl_TimeLogs");

            migrationBuilder.DropColumn(
                name: "TotalBreakPmHours",
                table: "tbl_TimeLogs");

            migrationBuilder.DropColumn(
                name: "TotalLunchHours",
                table: "tbl_TimeLogs");
        }
    }
}
