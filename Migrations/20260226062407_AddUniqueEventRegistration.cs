using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueEventRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistrations");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId_EPFNo",
                table: "EventRegistrations",
                columns: new[] { "EventId", "EPFNo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventRegistrations_EventId_EPFNo",
                table: "EventRegistrations");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistrations",
                column: "EventId");
        }
    }
}
