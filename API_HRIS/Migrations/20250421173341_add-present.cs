using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_HRIS.Migrations
{
    public partial class addpresent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DaysAbsent",
                table: "TblPayslipModel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DaysPresent",
                table: "TblPayslipModel",
                type: "int",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysAbsent",
                table: "TblPayslipModel");

            migrationBuilder.DropColumn(
                name: "DaysPresent",
                table: "TblPayslipModel");

        }
    }
}
