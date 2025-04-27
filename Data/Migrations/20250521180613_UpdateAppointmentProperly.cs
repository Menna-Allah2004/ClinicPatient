using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicPatient.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentProperly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVirtual",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MeetingLink",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVirtual",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "MeetingLink",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Appointments");
        }
    }
}
