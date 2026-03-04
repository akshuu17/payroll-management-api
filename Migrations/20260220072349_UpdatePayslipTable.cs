using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayslipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfJoining",
                table: "Payslips");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PayDate",
                table: "Payslips",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "BasicSalary",
                table: "Payslips",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "EPFNo",
                table: "Payslips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "NetSalary",
                table: "Payslips",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PF",
                table: "Payslips",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Tax",
                table: "Payslips",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TravellingAllowance",
                table: "Payslips",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicSalary",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "EPFNo",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "NetSalary",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "PF",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "TravellingAllowance",
                table: "Payslips");

            migrationBuilder.AlterColumn<string>(
                name: "PayDate",
                table: "Payslips",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DateOfJoining",
                table: "Payslips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
